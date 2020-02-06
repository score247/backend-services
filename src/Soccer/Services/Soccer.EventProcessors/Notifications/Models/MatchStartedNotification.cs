using Soccer.Core.Matches.Models;
using Soccer.Core.Teams.Models;

namespace Soccer.EventProcessors.Notifications.Models
{
    public class MatchStartedNotification : TimelineNotification
    {
        public MatchStartedNotification(
            TimelineEvent timeline,
            Team home,
            Team away) : base(timeline, home, away) { }

        protected override string Content() => $"{HomeTeam.Name} 0 : 0 {AwayTeam.Name}";

        protected override string Title() => "Match Started";
    }
}
