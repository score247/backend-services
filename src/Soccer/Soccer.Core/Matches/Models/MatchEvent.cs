using Soccer.Core.Shared.Enumerations;

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
        }

        public string LeagueId { get; }

        public string MatchId { get; }        

        public MatchResult MatchResult { get; }

        public TimelineEvent Timeline { get; }

        public MatchEvent AddScoreToSpecialTimeline(MatchResult matchResult) 
        {            
            if (Timeline.Type == EventType.BreakStart || Timeline.Type == EventType.MatchEnded)
            {
                Timeline.UpdateScore(matchResult.HomeScore, matchResult.AwayScore);
            }

            return this;
        }
    }
}