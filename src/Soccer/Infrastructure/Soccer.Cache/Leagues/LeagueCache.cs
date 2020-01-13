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

        Task ClearCountryLeaguesCache();

        Task<IEnumerable<League>> GetCountryLeagues(string countryCode, string language = Language.English);

        Task SetCountryLeagues(IEnumerable<League> countryLeagues, string countryCode, string language = Language.English);
    }

    public class LeagueCache : ILeagueCache
    {
        public const string MajorLeaguesCacheKey = "Major_Leagues";
        public const string CountryLeaguesCachePrefixKey = "Country_Leagues";
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

        public async Task ClearCountryLeaguesCache()
        {
            foreach (var language in Enumeration.GetAll<Language>())
            {
                await cacheManager.RemoveAsync(GetCountryLeaguesCacheKey(language.DisplayName));
            }
        }

        public async Task SetCountryLeagues(IEnumerable<League> countryLeagues, string countryCode, string language = Language.English)
        {
            var countryLeaguesCaches = await cacheManager.GetAsync<IDictionary<string, IEnumerable<League>>>(GetCountryLeaguesCacheKey(language))
                                       ?? new Dictionary<string, IEnumerable<League>>();

            if (countryLeaguesCaches.ContainsKey(countryCode))
            {
                countryLeaguesCaches[countryCode] = countryLeagues;
            }
            else
            {
                countryLeaguesCaches.Add(countryCode, countryLeagues);
            }

            await cacheManager.SetAsync(
                    GetCountryLeaguesCacheKey(language),
                    countryLeaguesCaches,
                    new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1)));
        }

        public async Task<IEnumerable<League>> GetCountryLeagues(string countryCode, string language = "en-US")
        {
            var countryLeaguesCaches = await cacheManager.GetAsync<IDictionary<string, IEnumerable<League>>>(GetCountryLeaguesCacheKey(language));

            if (countryLeaguesCaches?.ContainsKey(countryCode) == true)
            {
                return countryLeaguesCaches[countryCode];
            }

            return null;
        }

        private static string GetMajorLeaguesCacheKey(string language)
            => MajorLeaguesCacheKey + "_" + language;

        private static string GetCountryLeaguesCacheKey(string language)
            => CountryLeaguesCachePrefixKey + "_" + language;
    }
}