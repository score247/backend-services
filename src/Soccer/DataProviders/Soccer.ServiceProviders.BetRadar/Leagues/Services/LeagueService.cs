using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Logging;
using Refit;
using Score247.Shared.Enumerations;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders.Leagues;
using Soccer.DataProviders.SportRadar.Leagues.DataMappers;
using Soccer.DataProviders.SportRadar.Shared.Configurations;
using Soccer.DataProviders.SportRadar.Shared.Extensions;

namespace Soccer.DataProviders.SportRadar.Leagues.Services
{
    public interface ISportRadarLeagueApi
    {
        [Get("/soccer-{accessLevel}{version}/{region}/{language}/tournaments.json?api_key={apiKey}")]
        Task<Dtos.TournamentResult> GetRegionLeagues(string accessLevel, string version, string region, string language, string apiKey);
    }

    public class SportRadarLeagueService : ILeagueService
    {
        private readonly ISportRadarLeagueApi leagueApi;
        private readonly SportSettings soccerSettings;
        private readonly ILogger logger;

        public SportRadarLeagueService(ISportRadarLeagueApi leagueApi, ISportRadarSettings sportRadarSettings, ILogger logger)
        {
            this.leagueApi = leagueApi;
            soccerSettings = sportRadarSettings.Sports.FirstOrDefault(s => s.Id == Sport.Soccer.Value);
            this.logger = logger;
        }

        public async Task<IEnumerable<League>> GetLeagues(Language language)
        {
            try
            {
                var regions = soccerSettings.Regions;
                var leagues = new List<League>();

                foreach (var region in regions)
                {
                    var sportRadarLanguage = language.ToSportRadarFormat();
                    var regionLeaguesDto = await leagueApi.GetRegionLeagues(soccerSettings.AccessLevel, soccerSettings.Version, region.Name, sportRadarLanguage, region.Key);

                    var regionLeagues = regionLeaguesDto.tournaments.Select(league => LeagueMapper.MapLeague(league, region.Name)).ToList();

                    leagues.AddRange(regionLeagues);
                }

                return leagues;
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync(ex.ToString());
            }

            return new List<League>();
        }
    }
}