using System.Collections.Generic;
using Soccer.Core.Teams.Models;

namespace Soccer.Core.Leagues.Models
{
    public class GroupLog
    {
        public GroupLog(string id, IEnumerable<TeamLog> teams)
        {
            Id = id;
            Teams = teams;
        }

        public string Id { get; }

        public IEnumerable<TeamLog> Teams { get; }
    }
}