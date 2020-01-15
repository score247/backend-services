using Soccer.Core.Matches.Models;

namespace Soccer.Core.Matches.QueueMessages.MatchEvents
{
    public interface IInjuryTimeEventMessage
    {
        MatchEvent MatchEvent { get; }
    }

    public class InjuryTimeEventMessage : IInjuryTimeEventMessage
    {
        public InjuryTimeEventMessage(MatchEvent matchEvent)
        {
            MatchEvent = matchEvent;
        }

        public MatchEvent MatchEvent { get; }
    }
}