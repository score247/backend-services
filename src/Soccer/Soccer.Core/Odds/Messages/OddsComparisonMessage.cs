namespace Soccer.Core.Odds.Messages
{
    using System.Collections.Generic;
    using Soccer.Core.Odds.Models;

    public interface IOddsComparisonMessage
    {
        string MatchId { get; }

        IEnumerable<BetTypeOdds> BetTypeOddsList { get; }
    }

    public class OddsComparisonMessage : IOddsComparisonMessage
    {
        public OddsComparisonMessage(
            string matchId,
            IEnumerable<BetTypeOdds> betTypeOddsList)
        {
            MatchId = matchId;
            BetTypeOddsList = betTypeOddsList;
        }

        public string MatchId { get; }

        public IEnumerable<BetTypeOdds> BetTypeOddsList { get; }
    }
}