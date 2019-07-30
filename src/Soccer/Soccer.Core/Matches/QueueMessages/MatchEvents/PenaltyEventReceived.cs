namespace Soccer.Core.Matches.QueueMessages.MatchEvents
{
    using Soccer.Core.Matches.Models;

    public interface IPenaltyEventReceived
    {
        MatchEvent MatchEvent { get; }
    }

    public class PenaltyEventReceived : IPenaltyEventReceived
    {
        public PenaltyEventReceived(MatchEvent matchEvent)
        {
            MatchEvent = matchEvent;
        }

        public MatchEvent MatchEvent { get; }
    }
}