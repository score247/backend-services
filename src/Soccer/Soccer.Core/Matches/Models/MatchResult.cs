namespace Soccer.Core.Matches.Models
{
    using System.Collections.Generic;
    using Soccer.Core.Shared.Enumerations;

    public class MatchResult
    {
        // TODO: Move out
        public MatchStatus MatchStatus { get; set; }

        // TODO: Move out
        public MatchStatus EventStatus { get; set; }

        public int Period { get; set; }

        public IEnumerable<MatchPeriod> MatchPeriods { get; set; }

        public int MatchTime { get; set; }

        public string WinnerId { get; set; }

        public int HomeScore { get; set; }

        public int AwayScore { get; set; }

        public int AggregateHomeScore { get; set; }

        public int AggregateAwayScore { get; set; }

        public string AggregateWinnerId { get; set; }
    }
}