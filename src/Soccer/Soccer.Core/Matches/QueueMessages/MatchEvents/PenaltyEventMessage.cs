namespace Soccer.Core.Matches.QueueMessages.MatchEvents
{
    using Soccer.Core.Matches.Models;

    public interface IPenaltyEventMessage
    {
        MatchEvent MatchEvent { get; }
    }

    public class PenaltyEventMessage : IPenaltyEventMessage
    {
        public PenaltyEventMessage(MatchEvent matchEvent)
        {
            MatchEvent = matchEvent;
        }

        public MatchEvent MatchEvent { get; }
    }
}