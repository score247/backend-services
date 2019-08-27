namespace Soccer.Core.Matches.QueueMessages.MatchEvents
{
    using Soccer.Core.Matches.Models;

    public interface IPeriodStartEventMessage
    {
        MatchEvent MatchEvent { get; }
    }

    public class PeriodStartEventMessage : IPeriodStartEventMessage
    {
        public PeriodStartEventMessage(MatchEvent matchEvent)
        {
            MatchEvent = matchEvent;
        }

        public MatchEvent MatchEvent { get; }
    }
}