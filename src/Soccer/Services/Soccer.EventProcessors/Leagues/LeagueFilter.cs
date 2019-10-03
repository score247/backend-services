using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Caching;
using Fanex.Data.Repository;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Database.Leagues.Criteria;
using Soccer.Database.Matches.Criteria;
using Soccer.EventProcessors._Shared.Filters;
using Soccer.EventProcessors.Shared.Configurations;

namespace Soccer.EventProcessors.Leagues
{
    public class LeagueFilter :
        IFilter<IEnumerable<Match>, IEnumerable<Match>>,
        IFilter<Match, bool>,
        IFilter<MatchEvent, bool>
    {
        private const string MajorLeaguesCacheKey = "Major_Leagues";
        private readonly IDynamicRepository dynamicRepository;
        private readonly ICacheService cacheService;

        public LeagueFilter(IDynamicRepository dynamicRepository, ICacheService cacheService)
        {
            this.dynamicRepository = dynamicRepository;
            this.cacheService = cacheService;
        }

        public async Task<IEnumerable<Match>> FilterAsync(IEnumerable<Match> data)
        {
            var majorLeagues = await GetMajorLeagues();

            return data
                .Where(match => majorLeagues?.Any(league => league.Id == match.League.Id) == true)
                .Select(m => SetLeagueOrder(m, majorLeagues));
        }

        public Task<bool> FilterAsync(Match data)
        {
            // TODO: Implement later
            return Task.FromResult(true);
        }

        public async Task<bool> FilterAsync(MatchEvent data)
        {
            if (data == null)
            {
                return false;
            }

            var majorLeagues = await GetMajorLeagues();

            return majorLeagues?.Any(league => league.Id == data.LeagueId) == true;
        }

        private static Match SetLeagueOrder(Match match, IEnumerable<League> majorLeagues)
        {
            var matchLeague = majorLeagues.FirstOrDefault(l => l.Id == match.League.Id);
            match.League.SetOrder(matchLeague?.Order ?? 0);

            return match;
        }

        private async Task<IEnumerable<League>> GetMajorLeagues()
        {
            IEnumerable<League> majorLeagues = (await cacheService
                    .GetAsync<IEnumerable<League>>(MajorLeaguesCacheKey))?
                .ToList();

            if (majorLeagues?.Any() != true)
            {
                majorLeagues = await dynamicRepository.FetchAsync<League>(new GetActiveLeagueCriteria());
                await cacheService.SetAsync(
                    MajorLeaguesCacheKey,
                    majorLeagues,
                    new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1)));
            }

            return majorLeagues;
        }
    }
}