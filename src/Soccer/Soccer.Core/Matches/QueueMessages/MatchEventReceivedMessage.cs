namespace Soccer.Core.Matches.QueueMessages
{
    using Soccer.Core.Matches.Models;

    public interface IMatchEventReceivedMessage
    {
        MatchEvent MatchEvent { get; }
    }

    public class MatchEventReceivedMessage
    {
        public MatchEventReceivedMessage(MatchEvent matchEvent)
        {
            MatchEvent = matchEvent;
        }

        public MatchEvent MatchEvent { get; }
    }
}