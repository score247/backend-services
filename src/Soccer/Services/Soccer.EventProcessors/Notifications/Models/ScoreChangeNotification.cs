using Soccer.Core.Matches.Models;
using Soccer.Core.Teams.Models;

namespace Soccer.EventProcessors.Notifications.Models
{
    public class ScoreChangeNotification : TimelineNotification
    {
        private const string SOCCER_BALL_ICON = @"\xE2\x9A\xBD";

        public ScoreChangeNotification(
         TimelineEvent timeline,
         Team home,
         Team away,
         byte matchTime,
         MatchResult matchResult) : base(timeline, home, away, matchTime, matchResult) { }

        public override string Content()
        => $"{HomeTeam.Name} {MatchResult?.HomeScore} : {MatchResult?.AwayScore} {AwayTeam.Name}";

        public override string Title()
        => $"{SOCCER_BALL_ICON} GOAL!!! ({Timeline.MatchTime})";
    }
}
