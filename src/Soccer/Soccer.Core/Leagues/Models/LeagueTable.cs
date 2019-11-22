using System.Collections.Generic;
using Soccer.Core._Shared.Enumerations;

namespace Soccer.Core.Leagues.Models
{
    public class LeagueTable
    {
        public LeagueTable(
            League league,
            LeagueTableType type, 
            LeagueSeason leagueSeason, 
            IEnumerable<LeagueGroupTable> groupTables)
        {
            League = league;
            Type = type;
            LeagueSeason = leagueSeason;
            GroupTables = groupTables;
        }

        public League League { get; }

        public LeagueTableType Type { get; }

        public LeagueSeason LeagueSeason { get; }

        public IEnumerable<LeagueGroupTable> GroupTables { get; }
    }
}