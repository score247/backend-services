using System;
using Soccer.Core.Matches.Models;

namespace Soccer.EventProcessors.Matches.Filters
{
    public static class LiveMatchRangeValidator
    {
        //TODO refactor to use Range 
        private const int TimeSpanInMinutes = 10;
        private const int TimeSpanToForceRemove = 3;

        public static bool IsValidClosedMatch(Match match)
        {
            if (!match.MatchResult.EventStatus.IsClosed())
            {
                return false;
            }

            return match.LatestTimeline?.Time == null
                ? match.EventDate > DateTimeOffset.UtcNow.AddHours(-TimeSpanToForceRemove)
                : match.LatestTimeline.Time >= DateTimeOffset.UtcNow.AddMinutes(-TimeSpanInMinutes);
        }

        public static bool IsValidNotStartedMatch(Match match)
        => match.MatchResult.EventStatus.IsNotStart()
                && match.EventDate >= DateTimeOffset.UtcNow.AddMinutes(-TimeSpanInMinutes)
                && match.EventDate <= DateTimeOffset.UtcNow.AddMinutes(TimeSpanInMinutes);
    }
}
