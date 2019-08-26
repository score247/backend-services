namespace Soccer.EventPublishers.Odds.SignalR
{
    using System.Collections.Generic;
    using Soccer.Core.Odds.SignalREvents;

    internal class OddsMovementSignalRMessage
    {
        public OddsMovementSignalRMessage(byte sportId, string matchId, IEnumerable<OddsMovementEvent> oddsEvents)
        {
            SportId = sportId;
            MatchId = matchId;
            OddsEvents = oddsEvents;
        }

        public byte SportId { get; }

        public string MatchId { get; }

        public IEnumerable<OddsMovementEvent> OddsEvents { get; }
    }
}