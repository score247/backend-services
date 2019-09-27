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

            return data.Where(match => majorLeagues?.Any(league => league.Id == match.League.Id) == true);
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
            var match = await dynamicRepository.GetAsync<Match>(new GetMatchByIdCriteria(data.MatchId, Language.en_US));

            return majorLeagues?.Any(league => league.Id == match?.League?.Id) == true;
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