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
           string matchTime,
           MatchResult matchResult) : base(timeline, home, away, matchTime, matchResult) { }

        //TODO generate penalty shootout info
        protected override string Content()
            => $"{HomeTeam.Name} {MatchResult?.HomeScore} : {MatchResult?.AwayScore} {AwayTeam.Name}";

        protected override string Title()
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
    }
}
