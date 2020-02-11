using System.Linq;
using System.Text;
using Soccer.Core.Matches.Extensions;
using Soccer.Core.Matches.Models;
using Soccer.Core.Teams.Models;

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
        {
            var contentBuilder = new StringBuilder();
            contentBuilder.Append($"{HomeTeam.Name} {MatchResult?.HomeScore}");
            contentBuilder.Append(TeamSeparator);
            contentBuilder.Append($"{MatchResult?.AwayScore} {AwayTeam.Name}");
            contentBuilder.Append($"{NewLine}{GeneratePenaltyShootout()}");

            return contentBuilder.ToString();
        }

        public override string Title()
            => $"Match Ended {GenerateExtraPeriodTitle()}";

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

                return $"Penalty shoot-out: {HomeTeam.Name} {penaltyPeriod.HomeScore} : {penaltyPeriod.AwayScore} {AwayTeam.Name}";
            }

            return string.Empty;
        }
    }
}
