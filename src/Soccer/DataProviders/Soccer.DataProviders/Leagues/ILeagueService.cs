﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.DataProviders.Leagues
{
    public interface ILeagueService
    {
        Task<IEnumerable<League>> GetLeagues(Language language);

        Task<LeagueTable> GetLeagueStandings(string leagueId, Language language, string regionName);

        Task<LeagueTable> GetLeagueLiveStandings(string leagueId, Language language, string regionName);
    }

    public interface ILeagueScheduleService
    {
        Task<IEnumerable<Match>> GetLeagueMatches(string regionName, string leagueId, Language language);
    }

    public interface ILeagueSeasonService
    {
        Task<IEnumerable<LeagueSeasonProcessedInfo>> GetUnprocessedLeagueSeason();
    }
}