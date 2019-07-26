namespace Soccer.DataProviders.SportRadar.Leagues.Dtos
{
    public class TournamentRoundDto
    {
        public string type { get; set; }

        public int number { get; set; }

        public string name { get; set; }

        public int cup_round_match_number { get; set; }

        public int cup_round_matches { get; set; }

        public string other_match_id { get; set; }

        public string phase { get; set; }

        public string group { get; set; }
    }
}