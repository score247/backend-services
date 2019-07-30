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

        public async Task<bool> EventNotProcessed(MatchEvent matchEvent)
        {
            if (matchEvent == null)
            {
                return false;
            }

            var cacheKey = $"MatchPushEvent_Match_{matchEvent.MatchId}";
            var matchTimelines = cacheService.Get<IList<Timeline>>(cacheKey);

            if (matchTimelines == null)
            {
                matchTimelines = (await dynamicRepository.FetchAsync<Timeline>(new GetTimelineCriteria(matchEvent.MatchId))).ToList();
            }

            if (matchTimelines.Any(e => matchEvent.Timeline.Id == e.Id))
            {
                return false;
            }

            matchTimelines.Add(matchEvent.Timeline);
            await cacheService.SetAsync(cacheKey, matchTimelines, EventCacheOptions);

            return true;
        }
    }
}