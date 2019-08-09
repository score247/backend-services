namespace Soccer.Core.Odds.Messages
{
    using System.Collections.Generic;
    using Soccer.Core.Odds.Models;
    using Soccer.Core.Odds.SignalREvents;

    public interface IOddsChangeOnMatchEventMessage
    {
        string MatchId { get; }

        IEnumerable<OddsEvent> OddsEvents { get; }
    }

    public class OddsChangeOnMatchEventMessage : IOddsChangeOnMatchEventMessage
    {
        public OddsChangeOnMatchEventMessage(
            string matchId, 
            IEnumerable<OddsEvent> oddsEvents)
        {
            MatchId = matchId;
            OddsEvents = oddsEvents;
        }

        public string MatchId { get; }

        public IEnumerable<OddsEvent> OddsEvents { get; }
    }
}