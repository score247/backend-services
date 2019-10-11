using Soccer.Core.Matches.Extensions;

namespace Soccer.Core.Matches.Models
{
    public class MatchEvent
    {
        public MatchEvent(string leagueId, string matchId, MatchResult matchResult, TimelineEvent timeline, bool isLatest = true)
        {
            LeagueId = leagueId;
            MatchId = matchId;
            MatchResult = matchResult;
            Timeline = timeline;
            IsLatest = isLatest;
        }

        public string LeagueId { get; }

        public string MatchId { get; }

        public bool IsLatest { get; }

        public MatchResult MatchResult { get; }

        public TimelineEvent Timeline { get; }

        public MatchEvent AddScoreToSpecialTimeline(MatchResult matchResult) 
        {            
            if (Timeline.ShouldReprocessScore())
            {
                Timeline.UpdateScore(matchResult.HomeScore, matchResult.AwayScore);
            }

            return this;
        }
    }
}