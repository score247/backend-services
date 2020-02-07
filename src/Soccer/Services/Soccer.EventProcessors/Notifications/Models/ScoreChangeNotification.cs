using Soccer.Core.Matches.Models;
using Soccer.Core.Teams.Models;
using Soccer.EventProcessors.Notifications.Constants;

namespace Soccer.EventProcessors.Notifications.Models
{
    public class ScoreChangeNotification : TimelineNotification
    {
        public ScoreChangeNotification(
         TimelineEvent timeline,
         Team home,
         Team away,
         byte matchTime,
         MatchResult matchResult) : base(timeline, home, away, matchTime, matchResult) { }

        public override string Content()
        => $"{HomeTeam.Name} {MatchResult?.HomeScore} : {MatchResult?.AwayScore} {AwayTeam.Name}";

        public override string Title()
        => $"{EmojiConstants.SOCCER_BALL_ICON} GOAL!!! {MatchTimeDisplay}";
    }
}
