using System;
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

        Task<IEnumerable<MatchSummary>> GetTeamResults(string teamId, string opponentTeamId, Language language);
    }

    public class TeamQueryService : ITeamQueryService
    {
        private const int YearRange = 4;
        private readonly IDynamicRepository dynamicRepository;

        public TeamQueryService(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task<IEnumerable<MatchSummary>> GetHeadToHeads(string homeTeamId, string awayTeamId, Language language)
        {
            var criteria = new GetHeadToHeadsCriteria(homeTeamId, awayTeamId, language);
            var matches = await dynamicRepository.FetchAsync<Match>(criteria);

            return matches
                .Where(m => m.EventDate.Year >= DateTime.Now.Year - YearRange)
                .Select(m => new MatchSummary(m));
        }

        public async Task<IEnumerable<MatchSummary>> GetTeamResults(string teamId, string opponentTeamId, Language language)
        {
            var criteria = new GetTeamResultsCriteria(teamId, language);
            var matches = await dynamicRepository.FetchAsync<Match>(criteria);

            if (!string.IsNullOrEmpty(opponentTeamId))
            {
                matches = matches.Where(match => match.Teams.All(team => team.Id != opponentTeamId)
                                                 && match.MatchResult.EventStatus.IsClosed());
            }

            return matches
                .Where(m => m.EventDate.Year >= DateTime.Now.Year - YearRange)
                .Select(match => new MatchSummary(match));
        }
    }
}