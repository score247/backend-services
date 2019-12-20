using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Caching;
using Score247.Shared;
using Score247.Shared.Enumerations;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Cache.Leagues
{
    public interface ILeagueCache
    {
        Task<IEnumerable<League>> GetMajorLeagues(string language = Language.English);

        Task SetMajorLeagues(IEnumerable<League> majorLeagues, string language = Language.English);

        Task ClearMajorLeaguesCache();

        Task<IEnumerable<League>> GetCountryLeagues(string countryCode, string language = Language.English);

        Task SetCountryLeagues(IEnumerable<League> countryLeagues, string countryCode, string language = Language.English);
    }

    public class LeagueCache : ILeagueCache
    {
        public const string MajorLeaguesCacheKey = "Major_Leagues";
        public const string CountryLeaguesCachePrefixKey = "Country_Leagues_";
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

        public async Task ClearMajorLeaguesCache()
        {
            foreach (var language in Enumeration.GetAll<Language>())
            {
                await cacheManager.RemoveAsync(GetMajorLeaguesCacheKey(language.DisplayName));
            }
        }

        public Task SetCountryLeagues(IEnumerable<League> countryLeagues, string countryCode, string language = Language.English)
              => cacheManager.SetAsync(
                    GetCountryLeaguesCacheKey(countryCode, language),
                    countryLeagues,
                new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1)));

        public Task<IEnumerable<League>> GetCountryLeagues(string countryCode, string language = "en-US")
         => cacheManager.GetAsync<IEnumerable<League>>(GetCountryLeaguesCacheKey(countryCode, language));

        private static string GetMajorLeaguesCacheKey(string language)
            => MajorLeaguesCacheKey + "_" + language;

        private static string GetCountryLeaguesCacheKey(string countryCode, string language)
            => CountryLeaguesCachePrefixKey + countryCode + "_" + language;
    }
}