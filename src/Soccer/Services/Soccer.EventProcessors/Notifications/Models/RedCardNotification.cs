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
          ILanguageResourcesService languageResources,
          TimelineEvent timeline,
          Team home,
          Team away,
          byte matchTime,
          MatchResult matchResult) : base(languageResources, timeline, home, away, matchTime, matchResult) { }

        public override string Content(string language = Language.English)
            => string.Format(
                LanguageResources.GetString(NotificationRedCardPlayer, language),
                TeamReceived.Name,
                PlayerNameDisplay(language));

        public override string Title(string language = Language.English)
            => string.Format(
                LanguageResources.GetString(NotificationRedCard, language),
                MatchTimeDisplay);
    }
}