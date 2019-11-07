using System.Collections.Generic;
using System.Linq;
using Soccer.Core.Leagues.Models;

namespace Soccer.Core.Leagues.Extensions
{
    public static class LeagueExtension
    {
        public static void UpdateMajorLeagueInfo(this League league, IEnumerable<League> majorLeagues)
        {
            var majorLeague = majorLeagues.FirstOrDefault(l => l.Id == league.Id);

            if (majorLeague != null)
            {
                league.UpdateLeague(majorLeague.CountryCode, majorLeague.IsInternational, majorLeague.Order, majorLeague.Region);
            }
        }
    }
}