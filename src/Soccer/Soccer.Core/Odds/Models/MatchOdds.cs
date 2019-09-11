using MessagePack;

namespace Soccer.Core.Odds.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MessagePack;

    [MessagePackObject(keyAsPropertyName: true)]
    public class MatchOdds
    {
        public MatchOdds() { }

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

        public IEnumerable<BetTypeOdds> BetTypeOddsList { get; }

        [IgnoreMember]
        public DateTime? LastUpdated { get; private set; }

        public void SetLastUpdated(DateTime lastUpdated)
        {
            LastUpdated = lastUpdated;
        }

        public bool IsBetTypeOddsListEmpty()
            => BetTypeOddsList == null || !BetTypeOddsList.Any();
    }
}