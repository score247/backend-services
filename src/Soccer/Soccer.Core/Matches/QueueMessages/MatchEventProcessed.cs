namespace Soccer.Core.Matches.QueueMessages
{
    using Soccer.Core.Matches.Models;

    public interface IMatchEventProcessed
    {
        MatchEvent MatchEvent { get; }
    }

    public class MatchEventProcessed : IMatchEventProcessed
    {
        public MatchEventProcessed(MatchEvent matchEvent)
        {
            MatchEvent = matchEvent;
        }

        public MatchEvent MatchEvent { get; }
    }
}