using Fanex.Data.Repository;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Models;
using Soccer.Database.Leagues.Criteria;
using Soccer.EventProcessors._Shared.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Soccer.EventProcessors.Leagues
{
    public class LeagueFilter : IFilter<IEnumerable<Match>>, IFilter<Match>
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

        public async Task<Match> FilterAsync(Match data)
        {
            return data;
        }
    }
}