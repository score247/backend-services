using System.Collections.Generic;
using System.Threading.Tasks;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;

namespace Soccer.DataProviders.Teams.Services
{
    public interface ITeamHeadToHeadService
    {
        Task<IReadOnlyList<TeamHeadToHead>> GetTeamHeadToHeads(string homeTeamId, string awayTeamId, Language language);
    }
}