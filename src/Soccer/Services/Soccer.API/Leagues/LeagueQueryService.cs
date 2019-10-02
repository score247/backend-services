using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Caching;
using Fanex.Data.Repository;
using Soccer.API._Shared;
using Soccer.Core.Leagues.Models;
using Soccer.Database.Leagues.Criteria;

namespace Soccer.API.Leagues
{
    public interface ILeagueQueryService
    {
        Task<IEnumerable<League>> GetMajorLeagues();
    }

    public class LeagueQueryService : ILeagueQueryService
    {
        private const string MajorLeagueCacheKey = "LeagueQuery_MajorLeagueCacheKey";
        private readonly IDynamicRepository dynamicRepository;
        private readonly ICacheManager cacheManager;

        public LeagueQueryService(
            IDynamicRepository dynamicRepository,
            ICacheManager cacheManager)
        {
            this.dynamicRepository = dynamicRepository;
            this.cacheManager = cacheManager;
        }

        public async Task<IEnumerable<League>> GetMajorLeagues()
        {
            var majorLeagues = await cacheManager.GetOrSetAsync(
                MajorLeagueCacheKey,
                () => dynamicRepository.FetchAsync<League>(new GetActiveLeagueCriteria()),
                new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1)));

            return majorLeagues;
        }
    }
}