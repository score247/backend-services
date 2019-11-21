using System.Collections.Generic;
using Soccer.Core.Teams.Models;

namespace Soccer.Core.Leagues.Models
{
    public class GroupStanding
    {
        public GroupStanding(string id, string name, IEnumerable<TeamStanding> teamStandings)
        {
            Id = id;
            Name = name;
            TeamStandings = teamStandings;
        }

        public string Id { get; }

        public string Name { get; }

        public IEnumerable<TeamStanding> TeamStandings { get; }
    }
}