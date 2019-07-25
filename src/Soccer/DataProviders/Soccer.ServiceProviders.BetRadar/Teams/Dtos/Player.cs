namespace Soccer.DataProviders.SportRadar.Teams.Dtos
{
    public class Player
    {
        public string id { get; set; }

        public string name { get; set; }

        public int substituted_in { get; set; }

        public int substituted_out { get; set; }

        public int goals_scored { get; set; }

        public int assists { get; set; }

        public int own_goals { get; set; }

        public int yellow_cards { get; set; }

        public int yellow_red_cards { get; set; }

        public int red_cards { get; set; }

        public int goal_line_clearances { get; set; }

        public int interceptions { get; set; }

        public int chances_created { get; set; }

        public int crosses_successful { get; set; }

        public int crosses_total { get; set; }

        public int passes_short_successful { get; set; }

        public int passes_medium_successful { get; set; }

        public int passes_long_successful { get; set; }

        public int passes_short_total { get; set; }

        public int passes_medium_total { get; set; }

        public int passes_long_total { get; set; }

        public int duels_header_successful { get; set; }

        public int duels_sprint_successful { get; set; }

        public int duels_tackle_successful { get; set; }

        public int duels_header_total { get; set; }

        public int duels_sprint_total { get; set; }

        public int duels_tackle_total { get; set; }

        public int goals_conceded { get; set; }

        public int shots_faced_saved { get; set; }

        public int shots_faced_total { get; set; }

        public int penalties_faced { get; set; }

        public int penalties_saved { get; set; }

        public int fouls_committed { get; set; }

        public int was_fouled { get; set; }

        public int offsides { get; set; }

        public int shots_on_goal { get; set; }

        public int shots_off_goal { get; set; }

        public int shots_blocked { get; set; }

        public int minutes_played { get; set; }

        public float performance_score { get; set; }

        public int goals_by_head { get; set; }

        public int goals_by_penalty { get; set; }

        public int penalty_goals_scored { get; set; }
    }

    public class GoalScorer
    {
        public string id { get; set; }

        public string name { get; set; }

        public string method { get; set; }
    }

    public class Assist
    {
        public string id { get; set; }

        public string type { get; set; }

        public string name { get; set; }
    }
}