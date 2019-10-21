using System.Collections.Generic;
using System.Threading.Tasks;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.DataProviders.Leagues
{
    public interface ILeagueService
    {
        Task<IEnumerable<League>> GetLeagues(Language language);
    }
}