namespace Soccer.Core.Matches.Models
{
    using System.Collections.Generic;

    public class MatchEvent
    {
        public MatchResult MatchResult { get; set; }

        public IEnumerable<Timeline> TimeLines { get; set; }
    }
}