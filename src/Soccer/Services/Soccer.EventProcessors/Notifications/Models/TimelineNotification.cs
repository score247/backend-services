using Soccer.Core._Shared.Resources;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;

namespace Soccer.EventProcessors.Notifications.Models
{
    public abstract class TimelineNotification
    {
        private const string PlayerToBeDefined = "PlayerToBeDefined";

        protected const string TeamSeparator = " - ";
        protected const string NewLine = "\n";
        protected const int DefaultScore = 0;
        protected const string HomeTeamIdentifier = "home";

        protected TimelineNotification(
            ILanguageResourcesService languageResources,
            TimelineEvent timeline,
            Team home,
            Team away,
            byte matchTime = 0,
            MatchResult matchResult = null)
        {
            LanguageResources = languageResources;
            MatchTime = matchTime;
            Timeline = timeline;
            HomeTeam = home;
            AwayTeam = away;
            MatchResult = matchResult;
        }

        protected ILanguageResourcesService LanguageResources { get; }

        protected byte MatchTime { get; }

        protected TimelineEvent Timeline { get; }

        protected Team HomeTeam { get; }

        protected Team AwayTeam { get; }

        protected MatchResult MatchResult { get; }

        public abstract string Title(string language = Language.English);

        public abstract string Content(string language = Language.English);

        protected string MatchTimeDisplay
           => MatchTime == 0
           ? string.Empty
           : $"({Timeline.MatchTime}{StoppageTimeDisplay}')";

        protected string StoppageTimeDisplay
            => string.IsNullOrWhiteSpace(Timeline?.StoppageTime)
            ? string.Empty
            : $"+{Timeline.StoppageTime}";

        protected string PlayerNameDisplay(string language = Language.English)
            => string.IsNullOrWhiteSpace(Timeline.Player?.Name)
            ? LanguageResources.GetString(PlayerToBeDefined, language)
            : Timeline.Player.Name;

        protected Team TeamReceived
            => Timeline.Team == HomeTeamIdentifier 
            ? HomeTeam 
            : AwayTeam;
    }
}