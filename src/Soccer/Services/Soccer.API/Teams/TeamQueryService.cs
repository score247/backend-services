using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Database.Teams;

namespace Soccer.API.Teams
{
    public interface ITeamQueryService
    {
        Task<IEnumerable<MatchSummary>> GetHeadToHeads(string homeTeamId, string awayTeamId, Language language);
    }

    public class TeamQueryService : ITeamQueryService
    {
        private readonly IDynamicRepository dynamicRepository;

        public TeamQueryService(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task<IEnumerable<MatchSummary>> GetHeadToHeads(string homeTeamId, string awayTeamId, Language language)
        {
            var criteria = new GetHeadToHeadsCriteria(homeTeamId, awayTeamId, language);
            var matches = await dynamicRepository.FetchAsync<Match>(criteria);

            return matches.Select(m => new MatchSummary(m));
        }
    }
}