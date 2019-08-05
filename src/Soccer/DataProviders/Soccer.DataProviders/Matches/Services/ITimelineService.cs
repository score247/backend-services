namespace Soccer.DataProviders.Matches.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Shared.Enumerations;

    public interface ITimelineService
    {
        Task<IReadOnlyList<TimelineEvent>> GetTimelines(string matchId, string region, Language language);
    }
}
