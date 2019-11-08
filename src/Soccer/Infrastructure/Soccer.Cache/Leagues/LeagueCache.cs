using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Caching;
using Score247.Shared;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Cache.Leagues
{
    public interface ILeagueCache
    {
        Task<IEnumerable<League>> GetMajorLeagues(string language = Language.English);

        Task SetMajorLeagues(IEnumerable<League> majorLeagues, string language = Language.English);

        Task ClearMajorLeaguesCache(string language = Language.English);
    }

    public class LeagueCache : ILeagueCache
    {
        public const string MajorLeaguesCacheKey = "Major_Leagues";
        private readonly ICacheManager cacheManager;

        public LeagueCache(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
        }

        public Task SetMajorLeagues(IEnumerable<League> majorLeagues, string language = Language.English)
              => cacheManager.SetAsync(
                    GetMajorLeaguesCacheKey(language),
                    majorLeagues,
                new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1)));

        public Task<IEnumerable<League>> GetMajorLeagues(string language = Language.English)
            => cacheManager.GetAsync<IEnumerable<League>>(GetMajorLeaguesCacheKey(language));

        public Task ClearMajorLeaguesCache(string language = Language.English) => cacheManager.RemoveAsync(GetMajorLeaguesCacheKey(language));

        private static string GetMajorLeaguesCacheKey(string language) => MajorLeaguesCacheKey + "_" + language;
    }
}