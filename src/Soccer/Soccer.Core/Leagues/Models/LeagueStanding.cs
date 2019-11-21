using System.Collections.Generic;

namespace Soccer.Core.Leagues.Models
{
    public class LeagueStanding
    {
        public League League { get; }
        public LeagueSeason LeagueSeason { get; }
        public IEnumerable<GroupStanding> standings { get; }
        public IEnumerable<Note> notes { get; }
    }
}