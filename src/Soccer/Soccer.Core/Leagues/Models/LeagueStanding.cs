using System.Collections.Generic;

namespace Soccer.Core.Leagues.Models
{
    public class LeagueStanding
    {
        public LeagueStanding(
            League league, 
            LeagueSeason leagueSeason, 
            IEnumerable<GroupStanding> standings, 
            IEnumerable<Note> notes)
        {
            League = league;
            LeagueSeason = leagueSeason;
            Standings = standings;
            Notes = notes;
        }

        public League League { get; }

        public LeagueSeason LeagueSeason { get; }

        public IEnumerable<GroupStanding> Standings { get; }

        public IEnumerable<Note> Notes { get; }
    }
}