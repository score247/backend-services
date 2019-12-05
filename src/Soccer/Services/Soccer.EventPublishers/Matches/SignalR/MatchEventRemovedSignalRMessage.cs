namespace Soccer.EventPublishers.Matches.SignalR
{
    public class MatchEventRemovedSignalRMessage
    {
        public MatchEventRemovedSignalRMessage(string matchId, string[] timelineIds)
        {
            MatchId = matchId;
            TimelineIds = timelineIds;
        }

        public string MatchId { get; }

        public string[] TimelineIds { get; }
    }
}
