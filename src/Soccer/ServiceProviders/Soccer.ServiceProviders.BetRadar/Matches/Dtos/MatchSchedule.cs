namespace Soccer.DataProviders.SportRadar.Matches.Dtos
{
    using System;
    using System.Collections.Generic;
    using Soccer.DataProviders.SportRadar.Leagues.Dtos;

    public class MatchSchedule
    {
        public DateTime generated_at { get; set; }

        public string schema { get; set; }

        public IEnumerable<SportEvent> sport_events { get; set; }
    }

    public class SportEvent
    {
        public string id { get; set; }

        public DateTime scheduled { get; set; }

        public bool start_time_tbd { get; set; }

        public TournamentRound tournament_round { get; set; }

        public Season season { get; set; }

        public Tournament tournament { get; set; }

        public IEnumerable<Competitor> competitors { get; set; }

        public Venue venue { get; set; }
    }

    public class Competitor
    {
        public string id { get; set; }

        public string name { get; set; }

        public string country { get; set; }

        public string country_code { get; set; }

        public string abbreviation { get; set; }

        public string qualifier { get; set; }
    }
}