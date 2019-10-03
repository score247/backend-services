using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Caching;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Models;
using Soccer.Database.Leagues.Criteria;
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
            var internationalCode = appSettings.InternationalLeagues
                                        .FirstOrDefault(league => league.Id == match.League.Id)?.CountryCode;

            if (!string.IsNullOrWhiteSpace(internationalCode))
            {
                match.League.SetInternationalLeagueCode(internationalCode);
            }

            return match;
        }
    }
}