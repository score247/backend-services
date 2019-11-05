using Soccer.Core.Matches.Models;

namespace Soccer.Core.Matches.QueueMessages.MatchEvents
{
    public interface IBreakStartEventMessage
    {
        MatchEvent MatchEvent { get; }
    }

    public class BreakStartEventMessage : IBreakStartEventMessage
    {
        public BreakStartEventMessage(MatchEvent matchEvent)
        {
            MatchEvent = matchEvent;
        }

        public MatchEvent MatchEvent { get; }
    }
}
