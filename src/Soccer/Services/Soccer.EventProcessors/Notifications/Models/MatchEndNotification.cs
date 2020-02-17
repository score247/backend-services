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
        private const string MatchEnd = "MatchEnd";
        private const string AfterExtraTime = "AfterExtraTime";
        private const string AfterPenalty = "AfterPenalty";
        private const string PenaltyShootout = "PenaltyShootout";
        private const string Aggregate = "Aggregate";
        private const string Win = "Win";

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
                    contentBuilder.Append($". {GenerateAggregateWinner(language)}");
                }
            }

            return contentBuilder.ToString();
        }

        public override string Title(string language = Language.English)
            => $"{LanguageResources.GetString(MatchEnd, language)}{GenerateExtraPeriodTitle(language)}";

        private string GenerateExtraPeriodTitle(string language = Language.English)
        {
            if (MatchResult.IsEndedInMainTime())
            {
                return string.Empty;
            }

            return MatchResult.IsAfterExtra()
                ? $" {LanguageResources.GetString(AfterExtraTime, language)}"
                : $" {LanguageResources.GetString(AfterPenalty, language)}";
        }

        private string GeneratePenaltyShootout(string language = Language.English)
        {
            var penaltyPeriod = MatchResult.MatchPeriods.FirstOrDefault(period => period.PeriodType.IsPenalties());

            return NewLine + $"{LanguageResources.GetString(PenaltyShootout, language)} {penaltyPeriod.HomeScore}{TeamSeparator}{penaltyPeriod.AwayScore}";
        }

        private string GenerateAggregateInfo(string language = Language.English)
        {
            var aggregateWinnerName = GenerateAggregateWinnerName();

            if (string.IsNullOrWhiteSpace(aggregateWinnerName))
            {
                return string.Empty;
            }

            var aggInfo = NewLine
                + $"{LanguageResources.GetString(Aggregate, language)} {MatchResult.AggregateHomeScore}{TeamSeparator}{MatchResult.AggregateAwayScore}";

            return MatchResult.IsAfterPenaltyShootout()
                ? aggInfo
                : $"{aggInfo}. {GenerateAggregateWinner(language)}";
        }

        private string GenerateAggregateWinner(string language = Language.English)
        => $"{GenerateAggregateWinnerName()} {LanguageResources.GetString(Win, language)}";

        private string GenerateAggregateWinnerName()
        {
            if (string.IsNullOrWhiteSpace(MatchResult.AggregateWinnerId))
            {
                return string.Empty;
            }

            return MatchResult.AggregateWinnerId == HomeTeam.Id
                ? HomeTeam.Name
                : AwayTeam.Name;
        }
    }
}