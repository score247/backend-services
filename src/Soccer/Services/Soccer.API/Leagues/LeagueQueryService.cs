using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using Soccer.API.Shared.Configurations;
using Soccer.Cache.Leagues;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Database.Leagues.Criteria;

namespace Soccer.API.Leagues
{
    public interface ILeagueQueryService
    {
        Task<IEnumerable<League>> GetMajorLeagues(Language language);

        Task<IEnumerable<LeagueSeasonProcessedInfo>> GetLeagueSeasonFecth();

        Task<IEnumerable<MatchSummary>> GetMatches(string id, Language language);

        Task<LeagueTable> GetLeagueTable(string id, string seasonId, string groupName, Language language);
    }

    public class LeagueQueryService : ILeagueQueryService
    {
        private readonly IAppSettings appSetting;
        private readonly IDynamicRepository dynamicRepository;
        private readonly ILeagueCache leagueCache;

        public LeagueQueryService(
            IAppSettings appSetting,
            IDynamicRepository dynamicRepository,
            ILeagueCache leagueCache)
        {
            this.appSetting = appSetting;
            this.dynamicRepository = dynamicRepository;
            this.leagueCache = leagueCache;
        }

        public Task<IEnumerable<LeagueSeasonProcessedInfo>> GetLeagueSeasonFecth()
            => dynamicRepository.FetchAsync<LeagueSeasonProcessedInfo>(new GetUnprocessedLeagueSeasonCriteria());

        public async Task<LeagueTable> GetLeagueTable(string id, string seasonId, string groupName, Language language)
        {
            var leagueTable = await dynamicRepository.GetAsync<LeagueTable>(new GetLeagueTableCriteria(id, seasonId, language));

            return leagueTable;
        }

        public async Task<IEnumerable<League>> GetMajorLeagues(Language language)
        {
            var cachedMajorLeagues = (await leagueCache.GetMajorLeagues())?.ToList();

            if (cachedMajorLeagues?.Any() == false)
            {
                return cachedMajorLeagues;
            }

            var majorLeagues = (await dynamicRepository
                .FetchAsync<League>(new GetActiveLeaguesCriteria(language.DisplayName)))
                .ToList();
            await leagueCache.SetMajorLeagues(majorLeagues);

            return majorLeagues;
        }

        public async Task<IEnumerable<MatchSummary>> GetMatches(string id, Language language)
        {
            var matches = new List<MatchSummary>();

            var formerMatches = dynamicRepository
                .FetchAsync<Match>(new GetMatchesByLeagueCriteria(
                    id, 
                    language, 
                    DateTime.Now.AddDays(-appSetting.DatabaseQueryDateSpan)));

            var aheadMatches = dynamicRepository
                .FetchAsync<Match>(new GetMatchesByLeagueCriteria(
                    id, 
                    language, 
                    DateTime.Now.AddDays(appSetting.DatabaseQueryDateSpan)));

            var currentMatches = dynamicRepository
                .FetchAsync<Match>(new GetMatchesByLeagueCriteria(
                    id, 
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