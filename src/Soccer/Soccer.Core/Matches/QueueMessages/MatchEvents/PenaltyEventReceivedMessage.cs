namespace Soccer.Core.Matches.QueueMessages.MatchEvents
{
    using Soccer.Core.Matches.Models;

    public interface IPenaltyEventReceivedMessage
    {
        MatchEvent MatchEvent { get; }
    }

    public class PenaltyEventReceivedMessage : IPenaltyEventReceivedMessage
    {
        public PenaltyEventReceivedMessage(MatchEvent matchEvent)
        {
            MatchEvent = matchEvent;
        }

        public MatchEvent MatchEvent { get; }
    }
}