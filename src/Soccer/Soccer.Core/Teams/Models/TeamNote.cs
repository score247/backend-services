using System.Collections.Generic;
using Soccer.Core.Matches.Models;

namespace Soccer.Core.Teams.Models
{
    public class TeamNote
    {
        public string Id { get; }
        public string Name { get; }
        public IEnumerable<Commentary> Comments { get; }
    }
}