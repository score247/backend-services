using System;
using System.Collections.Generic;
using Soccer.DataProviders.SportRadar.Odds.Dtos;

namespace Soccer.DataProviders.SportRadar.Matches.Dtos
{
    public class MatchLineupsDto
    {
        public DateTime generated_at { get; set; }
        public string schema { get; set; }
        public SportEventDto sport_event { get; set; }
        public IList<Lineup> lineups { get; set; }
    }

    public class Lineup
    {
        public string team { get; set; }
        public string formation { get; set; }
        public Manager manager { get; set; }
        public Jersey jersey { get; set; }
        public IList<StartingLineup> starting_lineup { get; set; }
        public IList<Substitute> substitutes { get; set; }
    }

    public class Manager
    {
        public string id { get; set; }
        public string name { get; set; }
        public string nationality { get; set; }
        public string country_code { get; set; }
    }

    public class Jersey
    {
        public string type { get; set; }

        public string sleeve { get; set; }
        public string number { get; set; }
        public bool squares { get; set; }
        public bool stripes { get; set; }
        public bool horizontal_stripes { get; set; }
        public bool split { get; set; }
        public string shirt_type { get; set; }
        public string sleeve_detail { get; set; }
    }

    public class StartingLineup
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int jersey_number { get; set; }
        public string position { get; set; }
        public int order { get; set; }
    }

    public class Substitute
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int jersey_number { get; set; }
    }
}