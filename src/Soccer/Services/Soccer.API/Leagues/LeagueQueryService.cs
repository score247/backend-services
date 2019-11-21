﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using Soccer.Cache.Leagues;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Database.Leagues.Criteria;
using Soccer.Database.Matches.Criteria;

namespace Soccer.API.Leagues
{
    public interface ILeagueQueryService
    {
        Task<IEnumerable<League>> GetMajorLeagues(Language language);

        Task<IEnumerable<LeagueSeasonProcessedInfo>> GetLeagueSeasonFecth();

        Task<IEnumerable<MatchSummary>> GetMatches(string id, Language language);
    }

    public class LeagueQueryService : ILeagueQueryService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ILeagueCache leagueCache;

        public LeagueQueryService(
            IDynamicRepository dynamicRepository,
            ILeagueCache leagueCache)
        {
            this.dynamicRepository = dynamicRepository;
            this.leagueCache = leagueCache;
        }

        public Task<IEnumerable<LeagueSeasonProcessedInfo>> GetLeagueSeasonFecth()
        => dynamicRepository.FetchAsync<LeagueSeasonProcessedInfo>(new GetUnprocessedLeagueSeasonCriteria());

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
            var matches = await dynamicRepository.FetchAsync<Match>(new GetMatchesByLeagueCriteria(id, language));

            return matches.Select(m => new MatchSummary(m));
        }
    }
}