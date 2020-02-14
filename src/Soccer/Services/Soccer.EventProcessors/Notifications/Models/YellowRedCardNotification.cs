using Soccer.Core._Shared.Resources;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;

namespace Soccer.EventProcessors.Notifications.Models
{
    public class YellowRedCardNotification : TimelineNotification
    {
        private const string YellowRedCardPostfix = "YellowRedCardPostfix";
        private const string YellowRedCard = "YellowRedCard";

        public YellowRedCardNotification(
          ILanguageResourcesService languageResources,
          TimelineEvent timeline,
          Team home,
          Team away,
          byte matchTime,
          MatchResult matchResult) : base(languageResources, timeline, home, away, matchTime, matchResult) { }

        public override string Content(string language = Language.English)
            => $"{TeamReceived.Name}{TeamSeparator}{PlayerNameDisplay(language)} {LanguageResources.GetString(YellowRedCardPostfix, language)}";

        public override string Title(string language = Language.English)
            => $"{LanguageResources.GetString(YellowRedCard, language)} {MatchTimeDisplay}";
    }
}