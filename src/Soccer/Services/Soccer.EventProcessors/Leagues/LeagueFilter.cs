using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Caching;
using Fanex.Data.Repository;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Models;
using Soccer.Database.Leagues.Criteria;
using Soccer.EventProcessors._Shared.Cache;
using Soccer.EventProcessors._Shared.Filters;

namespace Soccer.EventProcessors.Leagues
{
    public class LeagueFilter :
        IAsyncFilter<IEnumerable<Match>, IEnumerable<Match>>,
        IAsyncFilter<MatchEvent, bool>,
        IAsyncFilter<Match, bool>
    {
        private const string MajorLeaguesCacheKey = "Major_Leagues";
        private readonly IDynamicRepository dynamicRepository;
        private readonly ICacheManager cacheManager;

        public LeagueFilter(IDynamicRepository dynamicRepository, ICacheManager cacheManager)
        {
            this.dynamicRepository = dynamicRepository;
            this.cacheManager = cacheManager;
        }

        public async Task<IEnumerable<Match>> Filter(IEnumerable<Match> data)
        {
            var majorLeagues = await GetMajorLeagues();

            return data
                .Where(match => majorLeagues?.Any(league => league.Id == match.League.Id) == true)
                .Select(m => SetLeagueOrder(m, majorLeagues));
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

        private static Match SetLeagueOrder(Match match, IEnumerable<League> majorLeagues)
        {
            var matchLeague = majorLeagues.FirstOrDefault(l => l.Id == match.League.Id);
            match.League.SetOrder(matchLeague?.Order ?? 0);

            return match;
        }

        private async Task<bool> IsBelongMajorLeagues(string leagueId)
        {
            var majorLeagues = await GetMajorLeagues();

            return majorLeagues?.Any(league => league.Id == leagueId) == true;
        }

        private async Task<IEnumerable<League>> GetMajorLeagues()
        {
            return await cacheManager.GetOrFetch(
                MajorLeaguesCacheKey,
                async () =>
                {
                    return await dynamicRepository.FetchAsync<League>(new GetActiveLeagueCriteria());
                },
                new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1)));
        }
    }
}