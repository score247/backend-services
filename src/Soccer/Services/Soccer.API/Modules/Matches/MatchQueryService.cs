namespace Soccer.API.Modules.Matches
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Soccer.Core.Domain.Matches;

    public interface MatchQueryService
    {
        Task<IEnumerable<Match>> GetMatches(int sportId, DateTime from, DateTime to, TimeSpan clientTimeZone, string language);

        Task<IEnumerable<Match>> GetLiveMatches(int sportId, string language);
    }
}