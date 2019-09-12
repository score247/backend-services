﻿namespace Soccer.Core.Odds.Models
{
    using System.Collections.Generic;
    using MessagePack;

    [MessagePackObject(keyAsPropertyName: true)]
    public class MatchOddsMovement
    {
        public MatchOddsMovement() { }

        [SerializationConstructor]
        public MatchOddsMovement(
            string matchId,
            Bookmaker bookmaker,
            IEnumerable<OddsMovement> oddsMovements)
        {
            MatchId = matchId;
            Bookmaker = bookmaker;
            OddsMovements = oddsMovements;
        }

        public string MatchId { get; private set; }

        public Bookmaker Bookmaker { get; private set; }

        public IEnumerable<OddsMovement> OddsMovements { get; private set; }
    }
}