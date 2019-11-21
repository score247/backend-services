using System.Collections.Generic;
using Soccer.Core.Teams.Models;

namespace Soccer.Core.Leagues.Models
{
    public class GroupNote
    {
        public string Id { get; }
        public IEnumerable<TeamNote> Teams { get; }
    }
}