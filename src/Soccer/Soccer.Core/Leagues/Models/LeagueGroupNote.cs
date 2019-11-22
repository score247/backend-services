using System.Collections.Generic;

namespace Soccer.Core.Leagues.Models
{
    public class LeagueGroupNote
    {
        public LeagueGroupNote(
            string teamId, 
            string teamName, 
            IEnumerable<string> comments)
        {
            TeamId = teamId;
            TeamName = teamName;
            Comments = comments;
        }

        public string TeamId { get; }

        public string TeamName { get; }

        public IEnumerable<string> Comments { get; }
    }
}