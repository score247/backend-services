namespace Soccer.Core.Matches.Models
{
    public class MatchEvent
    {
        public MatchEvent(string matchId, MatchResult matchResult, TimelineEvent timeline)
        {
            MatchId = matchId;
            MatchResult = matchResult;
            Timeline = timeline;
            Timeline.UpdateScore(matchResult.HomeScore, matchResult.AwayScore);
        }

        public string MatchId { get; }

        public MatchResult MatchResult { get; }

        public TimelineEvent Timeline { get; }
    }
}