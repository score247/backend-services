using Fanex.Data.Repository;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Models;
using Soccer.Database.Leagues.Criteria;
using Soccer.EventProcessors._Shared.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Soccer.Core.Shared.Enumerations;
using Soccer.Database.Matches.Criteria;

namespace Soccer.EventProcessors.Leagues
{
    public class LeagueFilter : IFilter<IEnumerable<Match>, IEnumerable<Match>>, IFilter<Match, bool>, IFilter<MatchEvent, bool>
    {
        private readonly IDynamicRepository dynamicRepository;

        public LeagueFilter(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task<IEnumerable<Match>> FilterAsync(IEnumerable<Match> data)
        {
            var activeLeagues = await dynamicRepository.FetchAsync<League>(new GetActiveLeagueCriteria());

            return data.Where(x => activeLeagues.Any(al => al.Id == x.League.Id));
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

            var activeLeagues = await dynamicRepository.FetchAsync<League>(new GetActiveLeagueCriteria());
            var match = await dynamicRepository.GetAsync<Match>(new GetMatchByIdCriteria(data.MatchId, Language.en_US));

            return activeLeagues.Any(x => x.Id == match?.League?.Id);
        }
    }
}