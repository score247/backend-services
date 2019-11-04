using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Caching;
using Fanex.Data.Repository;
using Score247.Shared;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Models;
using Soccer.Database.Leagues.Criteria;

namespace Soccer.EventProcessors.Leagues.Filters
{
    public interface IMajorLeagueFilter<in T, TResult>
    {
        Task<TResult> Filter(T data);
    }

    public class MajorLeagueFilter :
        IMajorLeagueFilter<IEnumerable<Match>, IEnumerable<Match>>,
        IMajorLeagueFilter<MatchEvent, bool>,
        IMajorLeagueFilter<Match, bool>,
        IMajorLeagueFilter<string, bool>
    {
        private const string MajorLeaguesCacheKey = "Major_Leagues";
        private readonly IDynamicRepository dynamicRepository;
        private readonly ICacheManager cacheManager;

        public MajorLeagueFilter(IDynamicRepository dynamicRepository, ICacheManager cacheManager)
        {
            this.dynamicRepository = dynamicRepository;
            this.cacheManager = cacheManager;
        }

        public async Task<IEnumerable<Match>> Filter(IEnumerable<Match> data)
        {
            var majorLeagues = await GetMajorLeagues();

            return data
                .Where(match => majorLeagues?.Any(league => league.Id == match.League.Id) == true)
                .Select(m => SetLeague(m, majorLeagues));
        }

        public async Task<bool> Filter(MatchEvent data)
        {
            if (data == null)
            {
                return false;
            }

            return await IsBelongMajorLeagues(data.LeagueId);
        }

        public async Task<bool> Filter(Match data)
        {
            if (data == null)
            {
                return false;
            }

            return await IsBelongMajorLeagues(data.League.Id);
        }

        public Task<bool> Filter(string data) 
            => IsBelongMajorLeagues(data);

        private static Match SetLeague(Match match, IEnumerable<League> majorLeagues)
        {
            var majorLeague = majorLeagues.FirstOrDefault(l => l.Id == match.League.Id);

            if (majorLeague != null)
            {
                match.League.UpdateLeague(majorLeague.CountryCode, majorLeague.IsInternational, majorLeague.Order, majorLeague.Region);
            }

            return match;
        }

        private async Task<bool> IsBelongMajorLeagues(string leagueId)
        {
            var majorLeagues = await GetMajorLeagues();

            return majorLeagues?.Any(league => league.Id == leagueId) == true;
        }

        private async Task<IEnumerable<League>> GetMajorLeagues()
        {
            return await cacheManager.GetOrSetAsync(
                MajorLeaguesCacheKey,
                async () => await dynamicRepository.FetchAsync<League>(new GetActiveLeaguesCriteria()),
                new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1)));
        }

       
    }
}