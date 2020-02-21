using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using Soccer.API.Shared.Configurations;
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
        private readonly IAppSettings appSetting;

        public TeamQueryService(IDynamicRepository dynamicRepository, IAppSettings appSetting)
        {
            this.dynamicRepository = dynamicRepository;
            this.appSetting = appSetting;
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

        public async Task<IEnumerable<TeamProfile>> SearchTeamByName(string keyword, Language language)
        {
            var pattern = @"\b" + keyword;
            var teams = await dynamicRepository.FetchAsync<TeamProfile>(new SearchTeamByNameCriteria(keyword, language));            

            var filterdTeam = teams
                .Where(team => System.Text.RegularExpressions.Regex.Match(
                            team.Name,
                            pattern, 
                            System.Text.RegularExpressions.RegexOptions.IgnoreCase).Success);

            return filterdTeam;
        }
        

        public async Task<IEnumerable<MatchSummary>> GetMatchesByTeam(string teamId, Language language)
        {
            var matches = new List<MatchSummary>();

            var formerMatches = dynamicRepository
                .FetchAsync<Match>(new GetMatchesByTeamCriteria(
                    teamId,
                    language,
                    DateTimeOffset.Now.AddDays(-appSetting.DatabaseQueryDateSpan)));

            var aheadMatches = dynamicRepository
                .FetchAsync<Match>(new GetMatchesByTeamCriteria(
                    teamId,
                    language,
                    DateTimeOffset.Now.AddDays(appSetting.DatabaseQueryDateSpan)));

            var currentMatches = dynamicRepository
                .FetchAsync<Match>(new GetMatchesByTeamCriteria(
                    teamId,
                    language));

            var results = await Task.WhenAll(currentMatches, aheadMatches, formerMatches);

            foreach (var result in results)
            {
                matches.AddRange(result.Select(m => new MatchSummary(m)));
            }

            return matches;
        }
       
    }
}