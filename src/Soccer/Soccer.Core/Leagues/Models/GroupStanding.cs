using System;
using System.Collections.Generic;
using System.Text;
using Soccer.Core.Teams.Models;

namespace Soccer.Core.Leagues.Models
{
    public class GroupStanding
    {
        public string Id { get; }

        public string Name { get; }
        public IEnumerable<TeamStanding> TeamStandings { get; set; }
    }
}