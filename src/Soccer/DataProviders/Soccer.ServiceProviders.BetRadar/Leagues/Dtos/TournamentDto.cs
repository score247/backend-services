namespace Soccer.DataProviders.SportRadar.Leagues.Dtos
{
    using System;
    using System.Collections.Generic;
    using Soccer.DataProviders.SportRadar.Matches.Dtos;

    public class TournamentResult
    {
        public DateTime generated_at { get; set; }

        public string schema { get; set; }

        public IEnumerable<TournamentDto> tournaments { get; set; }
    }

    public class TournamentDto
    {
        public string id { get; set; }

        public string name { get; set; }

        public SportDto sport { get; set; }

        public Category category { get; set; }

        public CurrentSeason current_season { get; set; }

        public SeasonCoverageInfo season_coverage_info { get; set; }
    }

    public class Category
    {
        public string id { get; set; }

        public string name { get; set; }

        public string country_code { get; set; }
    }

    public class SeasonDto
    {
        public string id { get; set; }

        public string name { get; set; }

        public string start_date { get; set; }

        public string end_date { get; set; }

        public string year { get; set; }

        public string tournament_id { get; set; }
    }

    public class SeasonCoverageInfo
    {
        public string season_id { get; set; }

        public int scheduled { get; set; }

        public int played { get; set; }

        public string max_coverage_level { get; set; }

        public int max_covered { get; set; }

        public string min_coverage_level { get; set; }
    }

    public class CurrentSeason
    {
        public string id { get; set; }

        public string name { get; set; }

        public string start_date { get; set; }

        public string end_date { get; set; }

        public string year { get; set; }
    }
}