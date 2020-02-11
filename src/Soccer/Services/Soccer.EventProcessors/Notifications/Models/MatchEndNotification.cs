using System.Linq;
using System.Text;
using Soccer.Core._Shared.Resources;
using Soccer.Core.Matches.Extensions;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;

namespace Soccer.EventProcessors.Notifications.Models
{
    public class MatchEndNotification : TimelineNotification
    {
        private const string NotificationMatchEnd = "NotificationMatchEnd";
        private const string NotificationAfterExtraTime = "NotificationAfterExtraTime";
        private const string NotificationAfterPenalty = "NotificationAfterPenalty";
        private const string NotificationMatchEndPenalty = "NotificationMatchEndPenalty";

        public MatchEndNotification(
           TimelineEvent timeline,
           Team home,
           Team away,
           byte matchTime,
           MatchResult matchResult) : base(timeline, home, away, matchTime, matchResult) { }

        public override string Content(string language = Language.English)
        {
            var contentBuilder = new StringBuilder();
            contentBuilder.Append($"{HomeTeam.Name} {MatchResult?.HomeScore}");
            contentBuilder.Append(TeamSeparator);
            contentBuilder.Append($"{MatchResult?.AwayScore} {AwayTeam.Name}");
            contentBuilder.Append($"{NewLine}{GeneratePenaltyShootout(language)}");

            return contentBuilder.ToString();
        }

        public override string Title(string language = Language.English)
            => string.Format(
                CustomAppResources.GetString(NotificationMatchEnd, language),
                GenerateExtraPeriodTitle(language));

        private string GenerateExtraPeriodTitle(string language = Language.English)
        {
            if (MatchResult.IsEndedInMainTime())
            {
                return string.Empty;
            }

            return MatchResult.IsAfterExtra()
                ? CustomAppResources.GetString(NotificationAfterExtraTime, language)
                : CustomAppResources.GetString(NotificationAfterPenalty, language);
        }

        private string GeneratePenaltyShootout(string language = Language.English)
        {
            if (MatchResult.IsAfterPenaltyShootout())
            {
                var penaltyPeriod = MatchResult.MatchPeriods.FirstOrDefault(period => period.PeriodType.IsPenalties());

                return string.Format(
                    CustomAppResources.GetString(NotificationMatchEndPenalty, language),
                    HomeTeam.Name,
                    penaltyPeriod.HomeScore,
                    penaltyPeriod.AwayScore,
                    AwayTeam.Name);
            }

            return string.Empty;
        }
    }
}