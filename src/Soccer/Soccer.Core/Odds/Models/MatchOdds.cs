namespace Soccer.Core.Odds.Models
{
    using System;
    using System.Collections.Generic;

    public class MatchOdds
    {
        public MatchOdds(
            string matchId,
            IEnumerable<BetTypeOdds> betTypeOddsList,
            DateTime? lastUpdated = null)
        {
            MatchId = matchId;
            LastUpdated = lastUpdated;
            BetTypeOddsList = betTypeOddsList;
        }

        public string MatchId { get; }

        public DateTime? LastUpdated { get; }

        public IEnumerable<BetTypeOdds> BetTypeOddsList { get; }
    }
}