namespace Soccer.EventPublishers.Odds.SignalR
{
    using System.Collections.Generic;
    using Soccer.Core.Odds.Models;

    internal class OddsComparisonSignalRMessage
    {
        public OddsComparisonSignalRMessage(byte sportId, string matchId, IEnumerable<BetTypeOdds> betTypeOddsList)
        {
            SportId = sportId;
            MatchId = matchId;
            BetTypeOddsList = betTypeOddsList;
        }

        public byte SportId { get; }

        public string MatchId { get; }

        public IEnumerable<BetTypeOdds> BetTypeOddsList { get; }
    }
}