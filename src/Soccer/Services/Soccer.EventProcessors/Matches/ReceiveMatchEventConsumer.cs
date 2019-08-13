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
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Database.Matches.Criteria;

    public class ReceiveMatchEventConsumer : IConsumer<IMatchEventReceivedMessage>
    {
        private static readonly CacheItemOptions EventCacheOptions =
            new CacheItemOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(10),
            };

        private static readonly IDictionary<int, int> PeriodStartTimeMapper =
            new Dictionary<int, int>
            {
                { MatchStatus.FirstHaft.Value, 1 },
                { MatchStatus.SecondHaft.Value, 46 },
                { MatchStatus.FirstHaftExtra.Value, 91 },
                { MatchStatus.SecondHaftExtra.Value, 106 }
            };

        private readonly ICacheService cacheService;
        private readonly IDynamicRepository dynamicRepository;
        private readonly IBus messageBus;

        public ReceiveMatchEventConsumer(ICacheService cacheService, IDynamicRepository dynamicRepository, IBus messageBus)
        {
            this.cacheService = cacheService;
            this.dynamicRepository = dynamicRepository;
            this.messageBus = messageBus;
        }

        public async Task Consume(ConsumeContext<IMatchEventReceivedMessage> context)
        {
            var matchEvent = context.Message?.MatchEvent;

            if (matchEvent != null && await IsTimelineEventNotProcessed(matchEvent))
            {
                if (matchEvent.Timeline.IsScoreChangeInPenalty())
                {
                    return;
                }

                GenerateMatchTime(matchEvent);

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

                if (matchEvent.Timeline.Type.IsRedCard() || matchEvent.Timeline.Type.IsYellowRedCard())
                {
                    await messageBus.Publish<IRedCardEventMessage>(new RedCardEventMessage(matchEvent));
                    return;
                }

                await messageBus.Publish<IMatchEventProcessedMessage>(new MatchEventProcessedMessage(matchEvent));
            }
        }

        private async Task<bool> IsTimelineEventProcessed(MatchEvent matchEvent)
        {
            var timelineEventsCacheKey = $"MatchPushEvent_Match_{matchEvent.MatchId}";

            var timeLineEvents = cacheService.Get<IList<TimelineEvent>>(timelineEventsCacheKey);

            if (timeLineEvents == null || timeLineEvents.Count == 0)
            {
                timeLineEvents = (await dynamicRepository.FetchAsync<TimelineEvent>
                    (new GetTimelineEventsCriteria(matchEvent.MatchId))).ToList();

                if (timeLineEvents?.Count > 0)
                {
                    await cacheService.SetAsync(timelineEventsCacheKey, timeLineEvents, EventCacheOptions);
                }
            }

            if (timeLineEvents?.Contains(matchEvent.Timeline) == true)
            {
                return true;
            }

            timeLineEvents.Add(matchEvent.Timeline);

            return false;
        }

        private async Task<bool> IsTimelineEventNotProcessed(MatchEvent matchEvent)
            => !await IsTimelineEventProcessed(matchEvent);

        private static void GenerateMatchTime(MatchEvent matchEvent)
        {
            if (matchEvent.MatchResult.EventStatus.IsLive())
            {
                var timeline = matchEvent.Timeline;
                var matchTime = timeline.MatchTime;
                var matchStatus = matchEvent.MatchResult.MatchStatus.Value;

                if (timeline.Type.IsPeriodStart() && PeriodStartTimeMapper.ContainsKey(matchStatus))
                {
                    matchTime = PeriodStartTimeMapper[matchStatus];
                }

                if (!string.IsNullOrEmpty(timeline.StoppageTime))
                {
                    matchTime += int.Parse(timeline.StoppageTime);
                }

                if (matchTime > 0)
                {
                    matchEvent.MatchResult.MatchTime = matchTime;
                }
            }
        }
    }
}