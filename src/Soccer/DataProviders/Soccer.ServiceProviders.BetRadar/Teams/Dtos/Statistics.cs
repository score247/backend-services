namespace Soccer.DataProviders.SportRadar.Teams.Dtos
{
    public class Statistics
    {
        public int ball_possession { get; set; }

        public int free_kicks { get; set; }

        public int throw_ins { get; set; }

        public int goal_kicks { get; set; }

        public int shots_blocked { get; set; }

        public int shots_on_target { get; set; }

        public int shots_off_target { get; set; }

        public int corner_kicks { get; set; }

        public int fouls { get; set; }

        public int shots_saved { get; set; }

        public int offsides { get; set; }

        public int yellow_cards { get; set; }

        public int injuries { get; set; }

        public int red_cards { get; set; }

        public int yellow_red_cards { get; set; }
    }
}