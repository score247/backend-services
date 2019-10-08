using System;
using System.Collections.Generic;
using System.Linq;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.EventProcessors._Shared.Filters;

namespace Soccer.EventProcessors.Matches.Filters
{
    public class MatchEventDateFilter : IFilter<IEnumerable<Match>, IEnumerable<Match>>
    {
        private const int TimeSpanInMinutes = 10;       

        public IEnumerable<Match> Filter(IEnumerable<Match> data)
            => data.Where(m => m.MatchResult.EventStatus == MatchStatus.Live
                                || IsValidNotStartMatch(m.MatchResult.EventStatus, m.EventDate)
                                || IsValidClosedMatch(m.MatchResult.EventStatus, m.LatestTimeline?.Time));

        private static bool IsValidNotStartMatch(MatchStatus eventStatus, DateTimeOffset dateTime)
            => eventStatus.IsNotStart()
                && dateTime >= DateTimeOffset.UtcNow.AddMinutes(-TimeSpanInMinutes)
                && dateTime <= DateTimeOffset.UtcNow.AddMinutes(TimeSpanInMinutes);

        private static bool IsValidClosedMatch(MatchStatus eventStatus, DateTimeOffset? dateTime)
            => (dateTime != null)
                && eventStatus.IsClosed()
                && dateTime >= DateTimeOffset.UtcNow.AddMinutes(-TimeSpanInMinutes);
    }
}
