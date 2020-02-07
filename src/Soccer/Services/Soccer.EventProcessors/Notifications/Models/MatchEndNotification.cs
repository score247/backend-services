using System.Linq;
using System.Text;
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
        {
            var strBuilder = new StringBuilder();
            strBuilder.Append($"{HomeTeam.Name} {MatchResult?.HomeScore}{HomeWinIcon}");
            strBuilder.Append($" : ");
            strBuilder.Append($"{MatchResult?.AwayScore} {AwayTeam.Name}{AwayWinIcon}");
            strBuilder.AppendLine(GeneratePenaltyShootout());

            return strBuilder.ToString();
        }

        public override string Title()
            => $"{EmojiConstants.ConvertIcon(EmojiConstants.FLAG_ICON)} Match Ended {GenerateExtraPeriodTitle()}";

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

        private string HomeWinIcon 
            => MatchResult.WinnerId == HomeTeam.Id 
            ? EmojiConstants.ConvertIcon(EmojiConstants.TROPHY_ICON) 
            : string.Empty;

        private string AwayWinIcon 
            => MatchResult.WinnerId == AwayTeam.Id 
            ? EmojiConstants.ConvertIcon(EmojiConstants.TROPHY_ICON) 
            : string.Empty;
    }
}
