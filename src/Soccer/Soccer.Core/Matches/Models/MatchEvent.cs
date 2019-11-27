using System;
using Soccer.Core.Matches.Extensions;

namespace Soccer.Core.Matches.Models
{
    public class MatchEvent
    {
        public MatchEvent(
            string leagueId, 
            string matchId, 
            MatchResult matchResult, 
            TimelineEvent timeline, 
            bool isLatest = true,
            DateTimeOffset eventDate = default)
        {
            LeagueId = leagueId;
            MatchId = matchId;
            MatchResult = matchResult;
            Timeline = timeline;
            IsLatest = isLatest;
            EventDate = eventDate;
        }

        public string LeagueId { get; }

        public string MatchId { get; }

        public bool IsLatest { get; }

        public MatchResult MatchResult { get; }

        public TimelineEvent Timeline { get; }

        public DateTimeOffset EventDate { get; }

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