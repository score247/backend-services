using Soccer.Core.Timelines.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.DataProviders.Matches.Services
{
    public interface ITimelineService
    {
        Task<Tuple<Match, IEnumerable<TimelineCommentary>>> GetTimelines(string matchId, string region, Language language);
    }
}