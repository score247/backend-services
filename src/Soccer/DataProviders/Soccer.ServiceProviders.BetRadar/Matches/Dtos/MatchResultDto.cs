namespace Soccer.DataProviders.SportRadar.Matches.Dtos
{
    using System;
    using System.Collections.Generic;

    public class MatchResultDto
    {
        public DateTime generated_at { get; set; }

        public string schema { get; set; }

        public IEnumerable<ResultDto> results { get; set; }
    }

    public class ResultDto
    {
        public SportEventDto sport_event { get; set; }

        public SportEventStatusDto sport_event_status { get; set; }
    }

    public class SportEventStatusDto
    {
        public string status { get; set; }

        public string match_status { get; set; }

        public int home_score { get; set; }

        public int away_score { get; set; }

        public string winner_id { get; set; }

        public int aggregate_home_score { get; set; }

        public int aggregate_away_score { get; set; }

        public string aggregate_winner_id { get; set; }

        public int period { get; set; }

        public ClockDto clock { get; set; }

        public IEnumerable<PeriodScoresDto> period_scores { get; set; }
    }

    public class ClockDto
    {
        public string match_time { get; set; }
    }

    public class PeriodScoresDto
    {
        public int home_score { get; set; }

        public int away_score { get; set; }

        public string type { get; set; }

        public int number { get; set; }
    }
}