namespace Soccer.Core.Matches.Models
{
    using MessagePack;
    using Soccer.Core.Shared.Enumerations;

#pragma warning disable S109 // Magic numbers should not be used
    [MessagePackObject]
    public class MatchPeriod
    {
        [Key(0)]
        public int HomeScore { get; set; }

        [Key(1)]
        public int AwayScore { get; set; }

        [Key(2)]
        public PeriodType PeriodType { get; set; }

        [Key(3)]
        public int Number { get; set; }
    }
#pragma warning restore S109 // Magic numbers should not be used
}