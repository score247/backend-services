using System.Collections.Generic;
using Soccer.Core.Teams.Models;

namespace Soccer.Core.Leagues.Models
{
    public class LeagueGroupTable
    {
        public LeagueGroupTable(
            string id, 
            string name,
            IEnumerable<LeagueGroupNote> groupNotes, 
            IEnumerable<TeamStanding> teamStandings)
        {
            Id = id;
            Name = name;
            GroupNotes = groupNotes;
            TeamStandings = teamStandings;
        }

        public string Id { get; }

        public string Name { get; }

        public IEnumerable<LeagueGroupNote> GroupNotes { get; }

        public IEnumerable<TeamStanding> TeamStandings { get; }
    }
}