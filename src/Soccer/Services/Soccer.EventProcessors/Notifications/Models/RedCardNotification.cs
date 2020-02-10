using Soccer.Core._Shared.Resources;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;

namespace Soccer.EventProcessors.Notifications.Models
{
    public class RedCardNotification : TimelineNotification
    {
        private const string NotificationRedCardPlayer = "NotificationRedCardPlayer";
        private const string NotificationRedCard = "NotificationRedCard";

        public RedCardNotification(
          TimelineEvent timeline,
          Team home,
          Team away,
          byte matchTime,
          MatchResult matchResult) : base(timeline, home, away, matchTime, matchResult) { }

        public override string Content(string language = Language.English)
            => string.Format(
                CustomAppResources.GetString(NotificationRedCardPlayer, language),
                TeamReceived.Name,
                PlayerNameDisplay(language));

        public override string Title(string language = Language.English)
            => string.Format(
                CustomAppResources.GetString(NotificationRedCard, language),
                EmojiConstants.ConvertIcon(EmojiConstants.RED_CARD_ICON),
                MatchTimeDisplay);

        private Team TeamReceived
            => Timeline.Team == "home" ? HomeTeam : AwayTeam;
    }
}