namespace Soccer.Core.Matches.Models
{
    using Soccer.Core.Shared.Enumerations;

    public class MatchPeriod
    {
        public int HomeScore { get; set; }

        public int AwayScore { get; set; }

        public PeriodType PeriodType { get; set; }

        public int Number { get; set; }
    }
}