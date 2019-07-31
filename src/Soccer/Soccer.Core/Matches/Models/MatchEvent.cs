namespace Soccer.Core.Matches.Models
{
    public class MatchEvent
    {
        public MatchEvent(string matchId, MatchResult matchResult, TimelineEventEntity timeline)
        {
            MatchId = matchId;
            MatchResult = matchResult;
            Timeline = timeline;
        }

        public string MatchId { get; }

        public MatchResult MatchResult { get; }

        public TimelineEventEntity Timeline { get; }
    }
}