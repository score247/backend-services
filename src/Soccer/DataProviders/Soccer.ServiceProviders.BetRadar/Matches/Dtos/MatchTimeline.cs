namespace Soccer.DataProviders.SportRadar.Matches.Dtos
{
    using System;
    using System.Collections.Generic;
    using Soccer.DataProviders.SportRadar.Teams.Dtos;

    public class MatchTimeline
    {
        public DateTime generated_at { get; set; }

        public string schema { get; set; }

        public SportEvent sport_event { get; set; }

        public SportEventStatus sport_event_status { get; set; }

        public IEnumerable<Timeline> timeline { get; set; }

        public Statistics statistics { get; set; }
    }

    public class Statistics
    {
        public IEnumerable<Team> teams { get; set; }
    }

    public class Timeline
    {
        public long id { get; set; }

        public string type { get; set; }

        public DateTime time { get; set; }

        public IEnumerable<Commentary> commentaries { get; set; }

        public int period { get; set; }

        public string period_type { get; set; }

        public string period_name { get; set; }

        public int match_time { get; set; }

        public string match_clock { get; set; }

        public string team { get; set; }

        public int x { get; set; }

        public int y { get; set; }

        public int home_score { get; set; }

        public int away_score { get; set; }

        public GoalScorer goal_scorer { get; set; }

        public Assist assist { get; set; }

        public Player player { get; set; }

        public string outcome { get; set; }

        public Player player_out { get; set; }

        public Player player_in { get; set; }

        public string stoppage_time { get; set; }

        public int injury_time_announced { get; set; }

        public string description { get; set; }

        public int? shootout_home_score { get; set; }

        public int? shootout_away_score { get; set; }

        public string status { get; set; }
    }

    public class Player
    {
        public string id { get; set; }

        public string name { get; set; }
    }

    public class Commentary
    {
        public string text { get; set; }
    }
}