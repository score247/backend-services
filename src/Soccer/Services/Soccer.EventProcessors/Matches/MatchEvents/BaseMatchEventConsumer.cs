namespace Soccer.EventProcessors.Matches.MatchEvents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Soccer.Core.Matches.Models;
    using Soccer.Database.Matches.Criteria;

    public abstract class BaseMatchEventConsumer
    {
        private static readonly CacheItemOptions EventCacheOptions =
           new CacheItemOptions
           {
               SlidingExpiration = TimeSpan.FromMinutes(10),
           };

        private readonly ICacheService cacheService;
        private readonly IDynamicRepository dynamicRepository;

        protected BaseMatchEventConsumer(ICacheService cacheService, IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
            this.cacheService = cacheService;
        }

        protected async Task<bool> IsTimelineEventProcessed(MatchEvent matchEvent)
        {
            if (matchEvent == null)
            {
                return true;
            }

            var timelineEventsCacheKey = $"MatchPushEvent_Match_{matchEvent.MatchId}";

            var timeLineEvents = cacheService.Get<IList<TimelineEvent>>(timelineEventsCacheKey);

            if (timeLineEvents == null || timeLineEvents.Count == 0)
            {
                timeLineEvents = (await dynamicRepository.FetchAsync<TimelineEvent>
                    (new GetTimelineCriteria(matchEvent.MatchId))).ToList();

                if (timeLineEvents?.Count > 0)
                {
                    await cacheService.SetAsync(timelineEventsCacheKey, timeLineEvents, EventCacheOptions);
                }
            }

            if (timeLineEvents?.Contains(matchEvent.Timeline) == true)
            {
                if (timeLineEvents.Contains(matchEvent.Timeline))
                {
                    return true;
                }

                timeLineEvents.Add(matchEvent.Timeline);
            }

            return false;
        }

        public async Task<bool> IsTimelineEventNotProcessed(MatchEvent matchEvent)
            => !await IsTimelineEventProcessed(matchEvent);
    }
}