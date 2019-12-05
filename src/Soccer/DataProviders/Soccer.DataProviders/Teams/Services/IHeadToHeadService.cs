using System.Collections.Generic;
using System.Threading.Tasks;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.DataProviders.Teams.Services
{
    public interface IHeadToHeadService
    {
        Task<IReadOnlyList<Match>> GetTeamHeadToHeads(string homeTeamId, string awayTeamId, Language language);

        Task<IReadOnlyList<Match>> GetTeamResults(string teamId, Language language);
    }
}