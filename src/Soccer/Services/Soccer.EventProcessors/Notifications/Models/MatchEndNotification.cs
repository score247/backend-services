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
        private const string NotificationMatchEndAggregate = "NotificationMatchEndAggregate";
        private const string NotificationAggregateWinner = "NotificationAggregateWinner";

        public MatchEndNotification(
           ILanguageResourcesService languageResources,
           TimelineEvent timeline,
           Team home,
           Team away,
           byte matchTime,
           MatchResult matchResult) : base(languageResources, timeline, home, away, matchTime, matchResult) { }

        public override string Content(string language = Language.English)
        {
            var contentBuilder = new StringBuilder();
            contentBuilder.Append($"{HomeTeam.Name} {MatchResult?.HomeScore}");
            contentBuilder.Append(TeamSeparator);
            contentBuilder.Append($"{MatchResult?.AwayScore} {AwayTeam.Name}");
            contentBuilder.Append(GenerateAggregateInfo(language));

            if (MatchResult.IsAfterPenaltyShootout())
            {
                contentBuilder.Append(GeneratePenaltyShootout(language));

                if (MatchResult.IsSecondLeg())
                {
                    contentBuilder.Append(GenerateAggregateWinner(language));
                }
            }

            return contentBuilder.ToString();
        }

        public override string Title(string language = Language.English)
            => string.Format(
                LanguageResources.GetString(NotificationMatchEnd, language),
                GenerateExtraPeriodTitle(language)).Trim();

        private string GenerateExtraPeriodTitle(string language = Language.English)
        {
            if (MatchResult.IsEndedInMainTime())
            {
                return string.Empty;
            }

            return MatchResult.IsAfterExtra()
                ? LanguageResources.GetString(NotificationAfterExtraTime, language)
                : LanguageResources.GetString(NotificationAfterPenalty, language);
        }

        private string GeneratePenaltyShootout(string language = Language.English)
        {
            var penaltyPeriod = MatchResult.MatchPeriods.FirstOrDefault(period => period.PeriodType.IsPenalties());

            return NewLine + string.Format(
                LanguageResources.GetString(NotificationMatchEndPenalty, language),
                penaltyPeriod.HomeScore,
                penaltyPeriod.AwayScore);
        }

        private string GenerateAggregateInfo(string language = Language.English)
        {
            var aggregateWinnerName = GenerateAggregateWinnerName(language);

            if (string.IsNullOrWhiteSpace(aggregateWinnerName))
            {
                return string.Empty;
            }

            var aggInfo = NewLine + string.Format(
               LanguageResources.GetString(NotificationMatchEndAggregate, language),
               MatchResult.AggregateHomeScore,
               MatchResult.AggregateAwayScore);

            return MatchResult.IsAfterPenaltyShootout()
                ? aggInfo 
                : $"{aggInfo}. {aggregateWinnerName}";
        }

        private string GenerateAggregateWinner(string language = Language.English)
        => NewLine + GenerateAggregateWinnerName(language);

        private string GenerateAggregateWinnerName(string language = Language.English)
        {
            if (string.IsNullOrWhiteSpace(MatchResult.AggregateWinnerId))
            {
                return string.Empty;
            }

            return MatchResult.AggregateWinnerId == HomeTeam.Id
                ? string.Format(LanguageResources.GetString(NotificationAggregateWinner, language), HomeTeam.Name)
                : string.Format(LanguageResources.GetString(NotificationAggregateWinner, language), AwayTeam.Name);
        }
    }
}