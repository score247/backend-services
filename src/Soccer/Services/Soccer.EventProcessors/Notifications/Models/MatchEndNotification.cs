using System;
using System.Linq;
using Soccer.Core.Matches.Extensions;
using Soccer.Core.Matches.Models;
using Soccer.Core.Teams.Models;
using Soccer.EventProcessors.Notifications.Constants;

namespace Soccer.EventProcessors.Notifications.Models
{
    public class MatchEndNotification : TimelineNotification
    {
        

        public MatchEndNotification(
           TimelineEvent timeline,
           Team home,
           Team away,
           byte matchTime,
           MatchResult matchResult) : base(timeline, home, away, matchTime, matchResult) { }

        public override string Content()
            => $"{HomeTeam.Name} {MatchResult?.HomeScore} : {MatchResult?.AwayScore} {AwayTeam.Name}" + GeneratePenaltyShootout();

        public override string Title()
            => $"{EmojiConstants.FLAG_ICON} Match Ended {GenerateExtraPeriodTitle()}";

        private string GenerateExtraPeriodTitle()
        {
            if (MatchResult.IsEndedInMainTime())
            {
                return string.Empty;
            }

            return MatchResult.IsAfterExtra()
                ? "after Extra Time"
                : "after Penalty Shoot-out";
        }

        private string GeneratePenaltyShootout() 
        {
            if (MatchResult.IsAfterPenaltyShootout())
            {
                var penaltyPeriod = MatchResult.MatchPeriods.FirstOrDefault(period => period.PeriodType.IsPenalties());

                return $"{Environment.NewLine}Penalty shoot-out: {HomeTeam.Name} {penaltyPeriod.HomeScore} : {penaltyPeriod.AwayScore} {AwayTeam.Name}";
            }

            return string.Empty;
        }
    }
}
