using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Caching;
using Fanex.Data.Repository;
using Fanex.Logging;
using MassTransit;
using Score247.Shared;
using Soccer.Core.Matches.Extensions;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Core.Matches.QueueMessages.MatchEvents;
using Soccer.Database.Matches.Criteria;

namespace Soccer.EventProcessors.Matches
{
    public class ReceiveMatchEventConsumer : IConsumer<IMatchEventReceivedMessage>
    {
        private static readonly CacheItemOptions EventCacheOptions = new CacheItemOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(10),
        };

        private readonly ICacheManager cacheManager;
        private readonly IDynamicRepository dynamicRepository;
        private readonly IBus messageBus;
        private readonly ILogger logger;

        public ReceiveMatchEventConsumer(
            ICacheManager cacheManager,
            IDynamicRepository dynamicRepository,
            IBus messageBus,
            ILogger logger)
        {
            this.cacheManager = cacheManager;
            this.dynamicRepository = dynamicRepository;
            this.messageBus = messageBus;
            this.logger = logger;
        }

#pragma warning disable S1541 // Methods and properties should not be too complex

        public async Task Consume(ConsumeContext<IMatchEventReceivedMessage> context)
#pragma warning restore S1541 // Methods and properties should not be too complex
        {
            var matchEvent = context.Message?.MatchEvent;

            if (matchEvent?.Timeline.IsScoreChangeInPenalty() != false)
            {
                return;
            }

            if (matchEvent.Timeline.Type.IsPeriodStart())
            {
                await messageBus.Publish<IPeriodStartEventMessage>(new PeriodStartEventMessage(matchEvent));
                return;
            }

            if (matchEvent.Timeline.IsShootOutInPenalty())
            {
                await messageBus.Publish<IPenaltyEventMessage>(new PenaltyEventMessage(matchEvent));
                return;
            }

            if (matchEvent.Timeline.Type.IsMatchEnd())
            {
                await messageBus.Publish<IMatchEndEventMessage>(new MatchEndEventMessage(matchEvent));
                return;
            }

            if (matchEvent.Timeline.Type.IsRedCard()
                || matchEvent.Timeline.Type.IsYellowRedCard()
                || matchEvent.Timeline.Type.IsYellowCard())
            {
                await InsertOrUpdateProcessedEvent(matchEvent);
                await messageBus.Publish<ICardEventMessage>(new CardEventMessage(matchEvent));
                return;
            }

            if (matchEvent.Timeline.Type.IsBreakStart())
            {
                await messageBus.Publish<IBreakStartEventMessage>(new BreakStartEventMessage(matchEvent));
                return;
            }

            if (matchEvent.Timeline.Type.IsInjuryTime())
            {
                await messageBus.Publish<IInjuryTimeEventMessage>(new InjuryTimeEventMessage(matchEvent));
            }

            await messageBus.Publish<IMatchEventProcessedMessage>(new MatchEventProcessedMessage(matchEvent));
        }

        private async Task<BlockingCollection<TimelineEvent>> GetProcessedTimelines(string matchId)
        {
            var timelineEventsCacheKey = $"MatchPushEvent_Match_{matchId}";

            var timelineEvents = await cacheManager.GetOrSetAsync(
                timelineEventsCacheKey,
                async () =>
                {
                    var timelineCollection = new BlockingCollection<TimelineEvent>();

                    var timelines = await dynamicRepository.FetchAsync<TimelineEvent>(new GetTimelineEventsCriteria(matchId));

                    foreach (var timeline in timelines)
                    {
                        timelineCollection.Add(timeline);
                    }

                    return timelineCollection;
                },
                EventCacheOptions);

            return timelineEvents;
        }

        private async Task InsertOrUpdateProcessedEvent(MatchEvent matchEvent)
        {
            var timeLineEvents = await GetProcessedTimelines(matchEvent.MatchId);

            var processedItem = timeLineEvents.FirstOrDefault(t => t.Id == matchEvent.Timeline.Id);

            try
            {
                if (processedItem != null)
                {
                    timeLineEvents.TryTake(out processedItem);
                }
            }
            catch (Exception e)
            {
                await logger.ErrorAsync($"Cannot remove item {processedItem?.Type} - {processedItem?.Team} of match {matchEvent.MatchId}", e);
            }

            timeLineEvents.Add(matchEvent.Timeline);
        }
    }
}