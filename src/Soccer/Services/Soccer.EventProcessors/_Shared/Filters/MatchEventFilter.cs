using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Database.Leagues.Criteria;
using Soccer.Database.Matches.Criteria;

namespace Soccer.EventProcessors._Shared.Filters
{
    public class MatchEventFilter : IFilter<MatchEvent>
    {
        private readonly IDynamicRepository dynamicRepository;

        public MatchEventFilter(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task<MatchEvent> FilterAsync(MatchEvent data)
        {
            var activeLeagues = await dynamicRepository.FetchAsync<League>(new GetActiveLeagueCriteria());

            var getmatchCriteria = new GetMatchByIdCriteria(data.MatchId, Language.en_US);
            var match = await dynamicRepository.GetAsync<Match>(getmatchCriteria);

            if (activeLeagues.Any(x => x.Id == match?.League?.Id))
            {
                return data;
            }

            return null;
        }
    }
}