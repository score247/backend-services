using System;
using System.Collections.Generic;
using System.Text;
using Soccer.Core.Matches.Models;
using Soccer.Core.Teams.Models;

namespace Soccer.EventProcessors.Notifications.Models
{
    public class YellowRedCardNotification : TimelineNotification
    {
        public YellowRedCardNotification(
          TimelineEvent timeline,
          Team home,
          Team away,
          byte matchTime,
          MatchResult matchResult) : base(timeline, home, away, matchTime, matchResult) { }

        public override string Content()
        => $"{TeamReceived.Name} - Player {Timeline.Player?.Name ?? "(TBD)"} 2nd yellow card and is off!";

        public override string Title() => $"Red Card {MatchTimeDisplay}";

        private Team TeamReceived => Timeline.Team == "home" ? HomeTeam : AwayTeam;
    }
}
