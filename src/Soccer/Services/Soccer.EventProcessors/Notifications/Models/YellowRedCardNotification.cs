using Soccer.Core.Matches.Models;
using Soccer.Core.Teams.Models;
using Soccer.EventProcessors.Notifications.Constants;

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
        => $"{TeamReceived.Name} - Player {PlayerNameDisplay} 2nd yellow card and is off!";

        public override string Title() => $"{EmojiConstants.ConvertIcon(EmojiConstants.RED_CARD_ICON)} Red Card {MatchTimeDisplay}";

        private Team TeamReceived => Timeline.Team == "home" ? HomeTeam : AwayTeam;
    }
}
