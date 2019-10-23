using System.Collections.Generic;

namespace Soccer.Core.Teams.Models
{
    public class TeamLineups : Team
    {
        public TeamLineups(
            string id,
            string name,
            string country,
            string countryCode,
            string flag,
            bool isHome,
            TeamStatistic statistic,
            Coach coach,
            string formation,
            string abbreviation,
            IEnumerable<Player> players,
            IEnumerable<Player> substitutions) : base(id, name, country, countryCode, flag, isHome, statistic, abbreviation)
        {
            Coach = coach;
            Formation = formation;
            Players = players;
            Substitutions = substitutions;
        }

        public Coach Coach { get; }

        public string Formation { get; }

        public IEnumerable<Player> Players { get; }

        public IEnumerable<Player> Substitutions { get; }
    }
}