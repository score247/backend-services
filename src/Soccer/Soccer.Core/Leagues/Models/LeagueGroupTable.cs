using System.Collections.Generic;
using Soccer.Core.Teams.Models;

namespace Soccer.Core.Leagues.Models
{
    public class LeagueGroupTable
    {
        public LeagueGroupTable(
            string id, 
            string name, 
            LeagueGroupNote groupNote, 
            IEnumerable<TeamStanding> teamStandings)
        {
            Id = id;
            Name = name;
            GroupNote = groupNote;
            TeamStandings = teamStandings;
        }

        public string Id { get; }

        public string Name { get; }

        public LeagueGroupNote GroupNote { get; }

        public IEnumerable<TeamStanding> TeamStandings { get; }
    }
}