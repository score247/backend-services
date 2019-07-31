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

            var cacheKey = $"MatchPushEvent_Match_{matchEvent.MatchId}";

            var timeLineList = cacheService.Get<IList<TimelineEventEntity>>(cacheKey);

            if (timeLineList == null || timeLineList.Count == 0)
            {
                timeLineList = (await dynamicRepository.FetchAsync<TimelineEventEntity>
                    (new GetTimelineCriteria(matchEvent.MatchId))).ToList();
            }

            if (timeLineList != null && timeLineList.Count > 0)
            {
                await cacheService.SetAsync(cacheKey, timeLineList, EventCacheOptions);

                if (timeLineList.Contains(matchEvent.Timeline))
                {
                    return true;
                }

                timeLineList.Add(matchEvent.Timeline);
            }

            return false;
        }

        public async Task<bool> IsTimelineEventNotProcessed(MatchEvent matchEvent)
            => !await IsTimelineEventProcessed(matchEvent);
    }
}