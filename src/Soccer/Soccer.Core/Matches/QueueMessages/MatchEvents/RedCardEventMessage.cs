namespace Soccer.Core.Matches.QueueMessages.MatchEvents
{
    using Soccer.Core.Matches.Models;

    public interface IRedCardEventMessage
    {
        MatchEvent MatchEvent { get; }
    }

    public class RedCardEventMessage : IRedCardEventMessage
    {
        public RedCardEventMessage(MatchEvent matchEvent)
        {
            MatchEvent = matchEvent;
        }

        public MatchEvent MatchEvent { get; }
    }
}