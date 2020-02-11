using Soccer.Core._Shared.Resources;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;

namespace Soccer.EventProcessors.Notifications.Models
{
    public class YellowRedCardNotification : TimelineNotification
    {
        private const string NotificationYellowRedCardPlayer = "NotificationYellowRedCardPlayer";
        private const string NotificationYellowRedCard = "NotificationYellowRedCard";

        public YellowRedCardNotification(
          TimelineEvent timeline,
          Team home,
          Team away,
          byte matchTime,
          MatchResult matchResult) : base(timeline, home, away, matchTime, matchResult) { }

        public override string Content(string language = Language.English)
            => string.Format(
                CustomAppResources.GetString(NotificationYellowRedCardPlayer, language),
                TeamReceived.Name,
                PlayerNameDisplay(language));

        public override string Title(string language = Language.English)
            => string.Format(
                CustomAppResources.GetString(NotificationYellowRedCard, language),
                MatchTimeDisplay);

        private Team TeamReceived
            => Timeline.Team == "home" ? HomeTeam : AwayTeam;
    }
}