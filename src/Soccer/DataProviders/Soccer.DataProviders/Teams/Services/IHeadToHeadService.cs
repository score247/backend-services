using System.Collections.Generic;
using System.Threading.Tasks;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;

namespace Soccer.DataProviders.Teams.Services
{
    public interface IHeadToHeadService
    {
        Task<IReadOnlyList<HeadToHead>> GetTeamHeadToHeads(string homeTeamId, string awayTeamId, Language language);
    }
}