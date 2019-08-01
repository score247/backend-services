namespace Soccer.DataProviders.Matches.Services
{
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Shared.Enumerations;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITimelineService
    {
        Task<IReadOnlyList<Timeline>> GetTimelines(string matchId, string region, Language language);
    }
}
