namespace Soccer.Core.Matches.Models
{
    public class MatchEvent
    {
        public MatchEvent(string leagueId, string matchId, MatchResult matchResult, TimelineEvent timeline)
        {
            LeagueId = leagueId;
            MatchId = matchId;
            MatchResult = matchResult;
            Timeline = timeline;
            Timeline.UpdateScore(matchResult.HomeScore, matchResult.AwayScore);
        }

        public string LeagueId { get; }

        public string MatchId { get; }

        public MatchResult MatchResult { get; }

        public TimelineEvent Timeline { get; }
    }
}