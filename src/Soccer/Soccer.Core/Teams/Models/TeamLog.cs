using System.Collections.Generic;
using Soccer.Core.Matches.Models;

namespace Soccer.Core.Teams.Models
{
    public class TeamLog
    {
        public string Id { get; }
        public string Name { get; }
        public IEnumerable<Commentary> Comments { get; }

        public TeamLog(
            string id,
            string name,
            IEnumerable<Commentary> comments)
        {
            Id = id;
            Name = name;
            Comments = comments;
        }
    }
}