namespace Soccer.DataProviders.SportRadar.Leagues.Dtos
{
    using System;
    using System.Collections.Generic;
    using Soccer.DataProviders.SportRadar.Matches.Dtos;
    using Soccer.DataProviders.SportRadar.Teams.Dtos;

    public class TournamentResult
    {
        public DateTime generated_at { get; set; }

        public string schema { get; set; }

        public IEnumerable<TournamentDto> tournaments { get; set; }
    }

    public class TournamentDetailDto
    {
        public DateTime generated_at { get; set; }

        public string schema { get; set; }

        public TournamentDto tournament { get; set; }

        public SeasonDto season { get; set; }

        public TournamentRoundDto round { get; set; }

        public SeasonCoverageInfo season_coverage_info { get; set; }

        public CoverageInfoDto coverage_info { get; set; }

        public IEnumerable<TournamentGroup> groups { get; set; }
    }

    public class TournamentSchedule
    {
        public DateTime generated_at { get; set; }

        public string schema { get; set; }

        public TournamentDto tournament { get; set; }

        public IEnumerable<SportEventDto> sport_events { get; set; }
    }

    public class TournamentDto
    {
        public string id { get; set; }

        public string name { get; set; }

        public SportDto sport { get; set; }

        public Category category { get; set; }

        public SeasonDto current_season { get; set; }

        public SeasonCoverageInfo season_coverage_info { get; set; }

        public CoverageInfoDto coverage_info { get; set; }

        public IEnumerable<TournamentGroup> groups { get; set; }
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

        public DateTime start_date { get; set; }

        public DateTime end_date { get; set; }

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

    public class TournamentGroup
    {
        public string id { get; set; }

        public string name { get; set; }

        public IEnumerable<Team> teams { get; set; }
    }
}