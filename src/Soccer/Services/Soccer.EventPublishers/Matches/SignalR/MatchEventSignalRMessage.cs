namespace Soccer.EventPublishers.Matches.SignalR
{
    using Soccer.Core.Matches.Models;

    internal class MatchEventSignalRMessage
    {
        public MatchEventSignalRMessage(byte sportId, MatchEvent matchEvent)
        {
            SportId = sportId;
            MatchEvent = matchEvent;
        }

        public byte SportId { get; }

        public MatchEvent MatchEvent { get; }
    }
}