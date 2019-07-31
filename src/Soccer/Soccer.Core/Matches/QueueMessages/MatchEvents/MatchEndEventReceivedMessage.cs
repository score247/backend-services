namespace Soccer.Core.Matches.QueueMessages.MatchEvents
{
    using Soccer.Core.Matches.Models;

    public interface IMatchEndEventReceivedMessage
    {
        MatchEvent MatchEvent { get; }
    }

    public class MatchEndEventReceivedMessage : IMatchEndEventReceivedMessage
    {
        public MatchEndEventReceivedMessage(MatchEvent matchEvent)
        {
            MatchEvent = matchEvent;
        }

        public MatchEvent MatchEvent { get; }
    }
}