namespace Soccer.Core.Odds.Messages
{
    using System.Collections.Generic;
    using Soccer.Core.Odds.SignalREvents;

    public interface IMatchEventOddsMessage
    {
        string MatchId { get; }

        IEnumerable<OddsEvent> OddsEvents { get; }
    }

    public class MatchEventOddsMessage : IMatchEventOddsMessage
    {
        public MatchEventOddsMessage(
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