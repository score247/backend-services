namespace Soccer.DataProviders.SportRadar.Matches.Dtos
{
    using System;
    using System.Collections.Generic;

    public class MatchResult
    {
        public DateTime generated_at { get; set; }

        public string schema { get; set; }

        public IEnumerable<Result> results { get; set; }
    }

    public class Result
    {
        public SportEvent sport_event { get; set; }

        public SportEventStatus sport_event_status { get; set; }
    }

    public class SportEventStatus
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

        public Clock clock { get; set; }

        public IEnumerable<PeriodScores> period_scores { get; set; }
    }

    public class Clock
    {
        public string match_time { get; set; }
    }

    public class PeriodScores
    {
        public int home_score { get; set; }

        public int away_score { get; set; }

        public string type { get; set; }

        public int number { get; set; }
    }
}