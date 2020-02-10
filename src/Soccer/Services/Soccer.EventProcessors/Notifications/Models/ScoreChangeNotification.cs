using System.Text;
using Soccer.Core.Matches.Models;
using Soccer.Core.Teams.Models;

namespace Soccer.EventProcessors.Notifications.Models
{
    public class ScoreChangeNotification : TimelineNotification
    {
        public ScoreChangeNotification(
         TimelineEvent timeline,
         Team home,
         Team away,
         byte matchTime,
         MatchResult matchResult) : base(timeline, home, away, matchTime, matchResult) { }

        public override string Content()
        {
            var contentBuilder = new StringBuilder();
            contentBuilder.Append($"{HomeTeam.Name} {BoundForScoredTeam(MatchResult?.HomeScore, HomeTeam.Id)}");
            contentBuilder.Append(TeamSeparator);
            contentBuilder.Append($"{BoundForScoredTeam(MatchResult?.AwayScore, AwayTeam.Id)} {AwayTeam.Name}");

            return contentBuilder.ToString();
        }

        public override string Title()
        => $"GOAL! {MatchTimeDisplay}";

        private string TeamScoredId => Timeline.Team == "home" ? HomeTeam.Id : AwayTeam.Id;

        private string BoundForScoredTeam(int? score, string teamId) 
            => TeamScoredId == teamId 
            ?  $"[{score?.ToString()}]" 
            : score?.ToString();
    }
}
