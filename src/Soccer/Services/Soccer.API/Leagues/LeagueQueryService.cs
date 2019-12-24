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

        Task<bool> CleanMajorLeaguesRequest();

        Task<IEnumerable<LeagueSeasonProcessedInfo>> GetLeagueSeasonFetch();

        Task<IEnumerable<MatchSummary>> GetMatches(string id, string leagueGroupName, Language language);

        Task<LeagueTable> GetLeagueTable(string id, string seasonId, string groupName, Language language);

        Task<IEnumerable<League>> GetCountryLeagues(string countryCode, Language language);

        Task<IEnumerable<LeagueGroupState>> GetLeagueGroups(string leagueId, string seasonId, Language language);
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

        public async Task<bool> CleanMajorLeaguesRequest()
        {
            await leagueCache.ClearMajorLeaguesCache();
            await leagueCache.ClearCountryLeaguesCache();

            return true;
        }

        public async Task<IEnumerable<League>> GetCountryLeagues(string countryCode, Language language)
        {
            var cachedMajorLeagues = (await leagueCache.GetCountryLeagues(countryCode, language.DisplayName))?.ToList();

            if (cachedMajorLeagues?.Any() == true)
            {
                return cachedMajorLeagues;
            }

            var countryLeagues = (await dynamicRepository
                .FetchAsync<League>(new GetCountryLeaguesCriteria(countryCode, language)))
                .ToList();
            await leagueCache.SetCountryLeagues(countryLeagues, countryCode, language.DisplayName);

            return countryLeagues;
        }

        public Task<IEnumerable<LeagueGroupState>> GetLeagueGroups(string leagueId, string seasonId, Language language)
            => dynamicRepository.FetchAsync<LeagueGroupState>(new GetLeagueGroupsCriteria(leagueId, seasonId, language));

        public Task<IEnumerable<LeagueSeasonProcessedInfo>> GetLeagueSeasonFetch()
            => dynamicRepository.FetchAsync<LeagueSeasonProcessedInfo>(new GetUnprocessedLeagueSeasonCriteria());

        public async Task<LeagueTable> GetLeagueTable(string id, string seasonId, string groupName, Language language)
        {
            var leagueTable = await dynamicRepository.GetAsync<LeagueTable>(new GetLeagueTableCriteria(id, seasonId, language));

            leagueTable?.FilterAndCalculateGroupTableOutcome(groupName);

            return leagueTable ?? new LeagueTable();
        }

        public async Task<IEnumerable<League>> GetMajorLeagues(Language language)
        {
            var cachedMajorLeagues = (await leagueCache.GetMajorLeagues())?.ToList();

            if (cachedMajorLeagues?.Any() == true)
            {
                return cachedMajorLeagues;
            }

            var majorLeagues = (await dynamicRepository
                .FetchAsync<League>(new GetActiveLeaguesCriteria(language.DisplayName)))
                .ToList();
            await leagueCache.SetMajorLeagues(majorLeagues);

            return majorLeagues;
        }

        public async Task<IEnumerable<MatchSummary>> GetMatches(string id, string leagueGroupName, Language language)
        {
            var matches = new List<MatchSummary>();

            var formerMatches = dynamicRepository
                .FetchAsync<Match>(new GetMatchesByLeagueCriteria(
                    id,
                    language,
                    DateTimeOffset.Now.AddDays(-appSetting.DatabaseQueryDateSpan)));

            var aheadMatches = dynamicRepository
                .FetchAsync<Match>(new GetMatchesByLeagueCriteria(
                    id,
                    language,
                    DateTimeOffset.Now.AddDays(appSetting.DatabaseQueryDateSpan)));

            var currentMatches = dynamicRepository
                .FetchAsync<Match>(new GetMatchesByLeagueCriteria(
                    id,
                    language));

            var results = await Task.WhenAll(currentMatches, aheadMatches, formerMatches);
            var league = (await GetMajorLeagues(language)).FirstOrDefault(league => league.Id == id);

            foreach (var result in results)
            {
                matches.AddRange(result.Select(m => new MatchSummary(m)).Where(m => m.LeagueSeasonId == league?.SeasonId));
            }

            if (!string.IsNullOrWhiteSpace(leagueGroupName))
            {
                return matches.Where(match => match.LeagueGroupName.Equals(leagueGroupName, StringComparison.InvariantCultureIgnoreCase));
            }

            return matches;
        }
    }
}