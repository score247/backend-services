using System.Collections.Generic;

namespace Soccer.Core.Leagues.Models
{
    public class LeagueTable
    {
        public LeagueTable(
            League league, 
            string type, 
            LeagueSeason leagueSeason, 
            IEnumerable<LeagueGroupTable> groupTables)
        {
            League = league;
            Type = type;
            LeagueSeason = leagueSeason;
            GroupTables = groupTables;
        }

        public League League { get; }

        public string Type { get; }

        public LeagueSeason LeagueSeason { get; }

        public IEnumerable<LeagueGroupTable> GroupTables { get; }
    }
}