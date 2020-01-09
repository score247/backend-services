namespace Soccer.Core.Matches.QueueMessages.MatchEvents
{
    using Soccer.Core.Matches.Models;

    public interface ICardEventMessage
    {
        MatchEvent MatchEvent { get; }
    }

    public class CardEventMessage : ICardEventMessage
    {
        public CardEventMessage(MatchEvent matchEvent)
        {
            MatchEvent = matchEvent;
        }

        public MatchEvent MatchEvent { get; }
    }
}