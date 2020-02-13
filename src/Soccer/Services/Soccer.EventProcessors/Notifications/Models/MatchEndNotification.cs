﻿using System.Linq;
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
            contentBuilder.Append($"{GeneratePenaltyShootout(language)}");
            contentBuilder.Append($"{GenerateAggregateInfo(language)}");

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
            if (MatchResult.IsAfterPenaltyShootout())
            {
                var penaltyPeriod = MatchResult.MatchPeriods.FirstOrDefault(period => period.PeriodType.IsPenalties());

                return NewLine + string.Format(
                    LanguageResources.GetString(NotificationMatchEndPenalty, language),
                    HomeTeam.Name,
                    penaltyPeriod.HomeScore,
                    penaltyPeriod.AwayScore,
                    AwayTeam.Name);
            }

            return string.Empty;
        }

        private string GenerateAggregateInfo(string language = Language.English)
        {
            if (string.IsNullOrWhiteSpace(MatchResult.AggregateWinnerId))
            {
                return string.Empty;
            }

            var aggregateWinnerName = MatchResult.AggregateWinnerId == HomeTeam.Id 
                ? HomeTeam.Name 
                : AwayTeam.Name;

            return NewLine + string.Format(
               LanguageResources.GetString(NotificationMatchEndAggregate, language),
               MatchResult.AggregateHomeScore,
               MatchResult.AggregateAwayScore,
               aggregateWinnerName);
        }
    }
}