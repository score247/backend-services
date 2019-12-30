namespace Soccer.Core.Timelines.QueueMessages
{
    public interface IMatchTimelinesRemovedMessage
    {
        string MatchId { get; }

        string[] TimelineIds { get; }
    }

    public class MatchTimelinesRemovedMessage : IMatchTimelinesRemovedMessage
    {
        public MatchTimelinesRemovedMessage(string matchId, string[] timelineIds)
        {
            MatchId = matchId;
            TimelineIds = timelineIds;
        }

        public string MatchId { get; }

        public string[] TimelineIds { get; }
    }
}
