namespace Soccer.EventProcessors.Matches
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Matches.Extensions;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Matches.QueueMessages;
    using Soccer.Core.Matches.QueueMessages.MatchEvents;
    using Soccer.Database.Matches.Criteria;
    using Soccer.EventProcessors._Shared.Filters;

    public class ReceiveMatchEventConsumer : IConsumer<IMatchEventReceivedMessage>
    {
        private static readonly CacheItemOptions EventCacheOptions = new CacheItemOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(10),
        };

        private readonly ICacheService cacheService;
        private readonly IDynamicRepository dynamicRepository;
        private readonly IBus messageBus;
        private readonly IFilter<MatchEvent, bool> matchEventFilter;

        public ReceiveMatchEventConsumer(ICacheService cacheService,
            IDynamicRepository dynamicRepository,
            IBus messageBus,
            IFilter<MatchEvent, bool> matchEventFilter)
        {
            this.cacheService = cacheService;
            this.dynamicRepository = dynamicRepository;
            this.messageBus = messageBus;
            this.matchEventFilter = matchEventFilter;
        }

        public async Task Consume(ConsumeContext<IMatchEventReceivedMessage> context)
        {
            var matchEvent = context.Message?.MatchEvent;
            var isValidMatchEvent = matchEvent != null
                                    && await matchEventFilter.FilterAsync(matchEvent);
            if (isValidMatchEvent)
            {
                if (matchEvent.Timeline.IsScoreChangeInPenalty())
                {
                    return;
                }

                if (matchEvent.Timeline.Type.IsPeriodStart())
                {
                    await messageBus.Publish<IPeriodStartEventMessage>(new PeriodStartEventMessage(matchEvent));
                    return;
                }

                if (await IsTimelineEventNotProcessed(matchEvent) && matchEvent.Timeline.IsShootOutInPenalty())
                {
                    await messageBus.Publish<IPenaltyEventMessage>(new PenaltyEventMessage(matchEvent));
                    return;
                }

                if (matchEvent.Timeline.Type.IsMatchEnd())
                {
                    await messageBus.Publish<IMatchEndEventMessage>(new MatchEndEventMessage(matchEvent));
                    return;
                }

                if (matchEvent.Timeline.Type.IsRedCard() || matchEvent.Timeline.Type.IsYellowRedCard())
                {
                    await InsertOrUpdateProcessedEvent(matchEvent);
                    await messageBus.Publish<IRedCardEventMessage>(new RedCardEventMessage(matchEvent));
                    return;
                }

                await messageBus.Publish<IMatchEventProcessedMessage>(new MatchEventProcessedMessage(matchEvent));
            }
        }

        private async Task<bool> IsTimelineEventProcessed(MatchEvent matchEvent)
        {
            var timeLineEvents = await GetProcessedTimelines(matchEvent.MatchId);

            if (timeLineEvents?.Contains(matchEvent.Timeline) == true)
            {
                return true;
            }

            timeLineEvents.Add(matchEvent.Timeline);

            return false;
        }

        private async Task<IList<TimelineEvent>> GetProcessedTimelines(string matchId)
        {
            IList<TimelineEvent> timelineEvents;

            var timelineEventsCacheKey = $"MatchPushEvent_Match_{matchId}";

            timelineEvents = cacheService.Get<IList<TimelineEvent>>(timelineEventsCacheKey);

            if (timelineEvents == null || timelineEvents.Count == 0)
            {
                timelineEvents = (await dynamicRepository.FetchAsync<TimelineEvent>
                    (new GetTimelineEventsCriteria(matchId))).ToList();

                if (timelineEvents?.Count > 0)
                {
                    await cacheService.SetAsync(timelineEventsCacheKey, timelineEvents, EventCacheOptions);
                }
            }

            return timelineEvents;
        }

        private async Task InsertOrUpdateProcessedEvent(MatchEvent matchEvent)
        {
            var timeLineEvents = await GetProcessedTimelines(matchEvent.MatchId);

            var processedItem = timeLineEvents.FirstOrDefault(t => t == matchEvent.Timeline);

            if (processedItem != null)
            {
                timeLineEvents.Remove(processedItem);
            }

            timeLineEvents.Add(matchEvent.Timeline);
        }

        private async Task<bool> IsTimelineEventNotProcessed(MatchEvent matchEvent)
            => !await IsTimelineEventProcessed(matchEvent);
    }
}