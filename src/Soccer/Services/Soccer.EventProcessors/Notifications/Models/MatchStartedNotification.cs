using Soccer.Core.Matches.Models;
using Soccer.Core.Teams.Models;

namespace Soccer.EventProcessors.Notifications.Models
{
    public class MatchStartedNotification : TimelineNotification
    {
        public MatchStartedNotification(
            TimelineEvent timeline,
            Team home,
            Team away,
            byte matchTime,
            MatchResult matchResult) 
            : base(timeline, home, away, matchTime, matchResult) { }

        public override string Content() => $"{HomeTeam.Name} 0{TeamSeparator}0 {AwayTeam.Name}";

        public override string Title() => $"Match Started";
    }
}
