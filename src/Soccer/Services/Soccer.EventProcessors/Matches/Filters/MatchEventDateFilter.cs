using System.Collections.Generic;
using System.Linq;
using Soccer.Core.Matches.Models;

namespace Soccer.EventProcessors.Matches.Filters
{
    public interface ILiveMatchFilter
    {
        IEnumerable<Match> FilterNotStarted(IEnumerable<Match> matches);

        IEnumerable<Match> FilterClosed(IEnumerable<Match> matches);
    }

    public class LiveMatchFilter : ILiveMatchFilter
    {
        private readonly ILiveMatchRangeValidator rangeValidator;

        public LiveMatchFilter(ILiveMatchRangeValidator rangeValidator)
        {
            this.rangeValidator = rangeValidator;
        }

        // remove invalid closed match
        public IEnumerable<Match> FilterClosed(IEnumerable<Match> matches)
         => matches.Where(m => m.MatchResult.EventStatus.IsLive()
                                || m.MatchResult.EventStatus.IsNotStart()
                                || rangeValidator.IsValidClosedMatch(m));

        // remove invalid not started match
        public IEnumerable<Match> FilterNotStarted(IEnumerable<Match> matches)
        => matches.Where(m => m.MatchResult.EventStatus.IsLive()
                                || rangeValidator.IsValidNotStartedMatch(m)
                                || m.MatchResult.EventStatus.IsClosed());
    }
}
