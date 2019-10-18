using System;
using Soccer.Core.Matches.Models;

namespace Soccer.EventProcessors.Matches.Filters
{
    public interface ILiveMatchRangeValidator
    {
        bool IsValidNotStartedMatch(Match match);

        bool IsValidClosedMatch(Match match);
    }

    public class LiveMatchRangeValidator : ILiveMatchRangeValidator
    {
        //TODO refactor to use Range 
        private const int TimeSpanInMinutes = 10;

        public bool IsValidClosedMatch(Match match)
        => match.MatchResult.EventStatus.IsClosed()
                && (match.LatestTimeline?.Time == null || match.LatestTimeline.Time >= DateTimeOffset.UtcNow.AddMinutes(-TimeSpanInMinutes));

        public bool IsValidNotStartedMatch(Match match)
        => match.MatchResult.EventStatus.IsNotStart()
                && match.EventDate >= DateTimeOffset.UtcNow.AddMinutes(-TimeSpanInMinutes)
                && match.EventDate <= DateTimeOffset.UtcNow.AddMinutes(TimeSpanInMinutes);
    }
}
