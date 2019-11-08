using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Caching;
using Score247.Shared;
using Soccer.Core.Leagues.Models;

namespace Soccer.Cache.Leagues
{
    public interface ILeagueCache
    {
        Task<IEnumerable<League>> GetMajorLeagues();

        Task SetMajorLeagues(IEnumerable<League> majorLeagues);

        Task ClearMajorLeaguesCache();
    }

    public class LeagueCache : ILeagueCache
    {
        public const string MajorLeaguesCacheKey = "Major_Leagues";
        private readonly ICacheManager cacheManager;

        public LeagueCache(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
        }

        public Task SetMajorLeagues(IEnumerable<League> majorLeagues)
          => cacheManager.SetAsync(
                MajorLeaguesCacheKey,
                majorLeagues,
            new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1)));

        public Task<IEnumerable<League>> GetMajorLeagues()
            => cacheManager.GetAsync<IEnumerable<League>>(MajorLeaguesCacheKey);

        public Task ClearMajorLeaguesCache() => cacheManager.RemoveAsync(MajorLeaguesCacheKey);
    }
}