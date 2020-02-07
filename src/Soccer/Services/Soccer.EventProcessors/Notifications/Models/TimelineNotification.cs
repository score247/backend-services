using Soccer.Core.Matches.Models;
using Soccer.Core.Teams.Models;

namespace Soccer.EventProcessors.Notifications.Models
{
    public abstract class TimelineNotification
    {
        protected const string TeamSeparator = " : ";

        protected TimelineNotification(
            TimelineEvent timeline, 
            Team home, 
            Team away,
            byte matchTime = 0,
            MatchResult matchResult = null) 
        {
            MatchTime = matchTime;
            Timeline = timeline;
            HomeTeam = home;
            AwayTeam = away;
            MatchResult = matchResult;
        }

        protected byte MatchTime { get; }

        protected TimelineEvent Timeline { get; }

        protected Team HomeTeam { get; }

        protected Team AwayTeam { get; }

        protected MatchResult MatchResult { get; }

        public abstract string Title();

        public abstract string Content();

        protected string MatchTimeDisplay
           => MatchTime == 0
           ? string.Empty
           : $"({Timeline.MatchTime}{StoppageTimeDisplay}')";

        protected string StoppageTimeDisplay
            => string.IsNullOrWhiteSpace(Timeline?.StoppageTime)
            ? string.Empty
            : $"+{Timeline?.StoppageTime}";

        protected string PlayerNameDisplay => string.IsNullOrWhiteSpace(Timeline.Player?.Name)
            ? "(TBD)"
            : Timeline.Player.Name;
    }
}
