using System.Collections.Generic;
using System.Threading.Tasks;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.DataProviders.Leagues
{
    public interface ILeagueService
    {
        Task<IEnumerable<League>> GetLeagues(Language language);

        Task<League> GetLeague(string leagueId, Language language);

        Task<League> GetLeagueStandings(string leagueId, Language language);

        Task<League> GetLeagueLiveStandings(string leagueId, Language language);
    }
}