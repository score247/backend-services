namespace Soccer.Core.Matches.QueueMessages.MatchEvents
{
    using Soccer.Core.Matches.Models;

    public interface INormalEventReceived
    {
        MatchEvent MatchEvent { get; }
    }

    public class NormalEventReceived : INormalEventReceived
    {
        public NormalEventReceived(MatchEvent matchEvent)
        {
            MatchEvent = matchEvent;
        }

        public MatchEvent MatchEvent { get; }
    }
}