namespace Soccer.DataProviders.SportRadar.Teams.Dtos
{
    using System.Collections.Generic;

    public class Team
    {
        public string id { get; set; }

        public string name { get; set; }

        public string country { get; set; }

        public string country_code { get; set; }

        public string abbreviation { get; set; }

        public string qualifier { get; set; }

        public Statistics statistics { get; set; }

        public IEnumerable<Player> players { get; set; }
    }
}