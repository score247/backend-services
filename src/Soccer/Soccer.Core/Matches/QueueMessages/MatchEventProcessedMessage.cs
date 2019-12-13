namespace Soccer.Core.Matches.QueueMessages
{
    using Soccer.Core.Matches.Models;

    public interface IMatchEventProcessedMessage
    {
        MatchEvent MatchEvent { get; }
    }

    public class MatchEventProcessedMessage : IMatchEventProcessedMessage
    {
        public MatchEventProcessedMessage(MatchEvent matchEvent)
        {
            MatchEvent = matchEvent;
        }

        public MatchEvent MatchEvent { get; private set; }
    }
}