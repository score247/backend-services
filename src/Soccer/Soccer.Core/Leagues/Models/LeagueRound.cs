﻿namespace Soccer.Core.Leagues.Models
{
    using Soccer.Core.Shared.Enumerations;

    public class LeagueRound
    {
        public LeagueRoundType Type { get; set; }

        public string Name { get; set; }

        public int Number { get; set; }

        public string Phase { get; set; }

        public string Group { get; set; }
    }
}