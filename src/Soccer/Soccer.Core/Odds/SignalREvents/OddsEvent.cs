namespace Soccer.Core.Odds.SignalREvents
{
    using Soccer.Core.Odds.Models;

    public class OddsEvent
    {
        public OddsEvent(
            int betTypeId,
            Bookmaker bookmaker,
            OddsMovement oddsMovement)
        {
            BetTypeId = betTypeId;
            Bookmaker = bookmaker;
            OddsMovement = oddsMovement;
        }

        public int BetTypeId { get; }

        public Bookmaker Bookmaker { get; }

        public OddsMovement OddsMovement { get; }
    }
}