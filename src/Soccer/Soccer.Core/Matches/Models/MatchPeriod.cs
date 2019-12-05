namespace Soccer.Core.Matches.Models
{
    using MessagePack;
    using Soccer.Core.Shared.Enumerations;

#pragma warning disable S109 // Magic numbers should not be used

    [MessagePackObject(keyAsPropertyName: true)]
    public class MatchPeriod
    {
        public int HomeScore { get; set; }

        public int AwayScore { get; set; }

        public PeriodType PeriodType { get; set; }

        public int Number { get; set; }
    }

#pragma warning restore S109 // Magic numbers should not be used
}