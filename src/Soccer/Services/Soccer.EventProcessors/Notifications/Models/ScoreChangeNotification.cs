using System.Text;
using Soccer.Core._Shared.Resources;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;

namespace Soccer.EventProcessors.Notifications.Models
{
    public class ScoreChangeNotification : TimelineNotification
    {
        private const string NotificationScoreChange = "NotificationScoreChange";

        public ScoreChangeNotification(
         ILanguageResourcesService languageResources,
         TimelineEvent timeline,
         Team home,
         Team away,
         byte matchTime,
         MatchResult matchResult) : base(languageResources, timeline, home, away, matchTime, matchResult) { }

        public override string Content(string language = Language.English)
        {
            var contentBuilder = new StringBuilder();
            contentBuilder.Append($"{HomeTeam.Name} {BoundForScoredTeam(MatchResult?.HomeScore, HomeTeam.Id)}");
            contentBuilder.Append(TeamSeparator);
            contentBuilder.Append($"{BoundForScoredTeam(MatchResult?.AwayScore, AwayTeam.Id)} {AwayTeam.Name}");

            return contentBuilder.ToString();
        }

        public override string Title(string language = Language.English)
            => string.Format(
                LanguageResources.GetString(NotificationScoreChange, language),
                MatchTimeDisplay);

        private string TeamScoredId
            => Timeline.Team == "home" ? HomeTeam.Id : AwayTeam.Id;

        private string BoundForScoredTeam(int? score, string teamId)
            => TeamScoredId == teamId
            ? $"[{score?.ToString()}]"
            : score?.ToString();
    }
}