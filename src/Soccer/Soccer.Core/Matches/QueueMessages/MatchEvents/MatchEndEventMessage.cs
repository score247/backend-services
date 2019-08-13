namespace Soccer.Core.Matches.QueueMessages.MatchEvents
{
    using Soccer.Core.Matches.Models;

    public interface IMatchEndEventMessage
    {
        MatchEvent MatchEvent { get; }
    }

    public class MatchEndEventMessage : IMatchEndEventMessage
    {
        public MatchEndEventMessage(MatchEvent matchEvent)
        {
            MatchEvent = matchEvent;
        }

        public MatchEvent MatchEvent { get; }
    }
}