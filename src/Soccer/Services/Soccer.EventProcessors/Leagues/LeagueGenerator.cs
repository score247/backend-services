using System.Linq;
using Soccer.Core.Matches.Models;
using Soccer.EventProcessors.Shared.Configurations;

namespace Soccer.EventProcessors.Leagues
{
    public interface ILeagueGenerator
    {
        Match GenerateInternationalCode(Match match);
    }

    public class LeagueGenerator : ILeagueGenerator
    {
        private readonly IAppSettings appSettings;

        public LeagueGenerator(IAppSettings appSettings)
        {
            this.appSettings = appSettings;
        }

        public Match GenerateInternationalCode(Match match)
        {
            var internationalLeague = appSettings.InternationalLeagues
                    .FirstOrDefault(league => league.Id == match.League.Id);

            if (internationalLeague != null)
            {
                match.League.SetInternationalLeagueCode(internationalLeague.CountryCode);
            }

            return match;
        }
    }
}