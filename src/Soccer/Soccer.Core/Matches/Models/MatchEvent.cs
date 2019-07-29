namespace Soccer.Core.Matches.Models
{
    public class MatchEvent
    {
        public string MatchId { get; set; }

        public MatchResult MatchResult { get; set; }

        public Timeline Timeline { get; set; }
    }
}