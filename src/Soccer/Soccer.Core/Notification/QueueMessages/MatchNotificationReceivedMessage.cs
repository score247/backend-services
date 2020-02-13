using Soccer.Core.Matches.Models;

namespace Soccer.Core.Notification.QueueMessages
{
    public interface IMatchNotificationReceivedMessage
    {
        string MatchId { get; }

        string LeagueId { get; }

        TimelineEvent Timeline { get; }

        MatchResult MatchResult { get; }
    }

    public class MatchNotificationReceivedMessage : IMatchNotificationReceivedMessage
    {
        public MatchNotificationReceivedMessage(string matchId, string leagueId, TimelineEvent timeline, MatchResult matchResult) 
        {
            MatchId = matchId;
            LeagueId = leagueId;
            Timeline = timeline;
            MatchResult = matchResult;
        }

        public string MatchId { get; }

        public string LeagueId { get; }

        public TimelineEvent Timeline { get; private set; }

        public MatchResult MatchResult { get; }
    }
}
