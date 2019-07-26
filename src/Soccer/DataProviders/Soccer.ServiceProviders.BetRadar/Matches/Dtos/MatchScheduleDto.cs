namespace Soccer.DataProviders.SportRadar.Matches.Dtos
{
    using System;
    using System.Collections.Generic;
    using Soccer.DataProviders.SportRadar.Leagues.Dtos;

    public class MatchScheduleDto
    {
        public DateTime generated_at { get; set; }

        public string schema { get; set; }

        public IEnumerable<SportEventDto> sport_events { get; set; }
    }

    public class SportEventDto
    {
        public string id { get; set; }

        public DateTime scheduled { get; set; }

        public bool start_time_tbd { get; set; }

        public TournamentRoundDto tournament_round { get; set; }

        public SeasonDto season { get; set; }

        public TournamentDto tournament { get; set; }

        public IEnumerable<CompetitorDto> competitors { get; set; }

        public VenueDto venue { get; set; }
    }

    public class CompetitorDto
    {
        public string id { get; set; }

        public string name { get; set; }

        public string country { get; set; }

        public string country_code { get; set; }

        public string abbreviation { get; set; }

        public string qualifier { get; set; }
    }
}