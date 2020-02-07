using Soccer.Core.Matches.Models;
using Soccer.Core.Teams.Models;

namespace Soccer.EventProcessors.Notifications.Models
{
    public class MatchStartedNotification : TimelineNotification
    {
        private const string SOUND_ICON = "U+1F50A";

        public MatchStartedNotification(
            TimelineEvent timeline,
            Team home,
            Team away) : base(timeline, home, away) { }

        public override string Content() => $"{HomeTeam.Name} 0 : 0 {AwayTeam.Name}";

        public override string Title() => $"{SOUND_ICON} Match Started";
    }
}
