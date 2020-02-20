using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;
using Soccer.Database.Teams;
using Soccer.Database.Teams.Criteria;

namespace Soccer.API.Teams
{
    public interface ITeamQueryService
    {
        Task<IEnumerable<MatchSummary>> GetHeadToHeads(string homeTeamId, string awayTeamId, Language language);

        Task<IEnumerable<MatchSummary>> GetTeamResults(string teamId, string opponentTeamId, Language language);

        Task<IEnumerable<TeamProfile>> SearchTeamByName(string keyword, Language language);

        Task<IEnumerable<TeamProfile>> GetTrendingTeams(Language language);

        Task<IEnumerable<MatchSummary>> GetMatchesByTeam(string teamId, Language language);
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

        public Task<IEnumerable<TeamProfile>> GetTrendingTeams(Language language)
            => dynamicRepository.FetchAsync<TeamProfile>(new GetTrendingTeamsCriteria(language));

        public Task<IEnumerable<TeamProfile>> SearchTeamByName(string keyword, Language language)
            => dynamicRepository.FetchAsync<TeamProfile>(new SearchTeamByNameCriteria(keyword, language));

        public Task<IEnumerable<MatchSummary>> GetMatchesByTeam(string teamId, Language language)
            => dynamicRepository.FetchAsync<MatchSummary>(new GetMatchesByTeamCriteria(teamId, language));
    }
}