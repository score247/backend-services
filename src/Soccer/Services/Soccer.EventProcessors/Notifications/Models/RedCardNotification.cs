using Soccer.Core.Matches.Models;
using Soccer.Core.Teams.Models;

namespace Soccer.EventProcessors.Notifications.Models
{
    public class RedCardNotification : TimelineNotification
    {
        public RedCardNotification(
          TimelineEvent timeline,
          Team home,
          Team away,
          byte matchTime,
          MatchResult matchResult) : base(timeline, home, away, matchTime, matchResult) { }

        public override string Content()
        => $"{TeamReceived.Name} - Player {Timeline.Player?.Name} has received a red card.";

        public override string Title() => $"Red Card ({Timeline.MatchTime})";

        private Team TeamReceived => Timeline.Team == "home" ? HomeTeam : AwayTeam;
    }
}
