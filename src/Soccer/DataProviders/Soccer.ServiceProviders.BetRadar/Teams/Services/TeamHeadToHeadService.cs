using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Logging;
using Refit;
using Score247.Shared.Enumerations;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;
using Soccer.DataProviders.SportRadar.Shared.Configurations;
using Soccer.DataProviders.SportRadar.Shared.Extensions;
using Soccer.DataProviders.SportRadar.Teams.DataMappers;
using Soccer.DataProviders.SportRadar.Teams.Dtos;
using Soccer.DataProviders.Teams.Services;

namespace Soccer.DataProviders.SportRadar.Teams.Services
{
    public interface ITeamHeadToHeadApi
    {
        [Get("/soccer-{accessLevel}{version}/{region}/{language}/teams/{homeTeamId}/versus/{awayTeamId}/matches.json?api_key={apiKey}")]
        Task<TeamHeadToHeadsDto> GetHeadToHead(string accessLevel, string version, string region, string language, string homeTeamId, string awayTeamId, string apiKey);
    }

    public class TeamHeadToHeadService : ITeamHeadToHeadService
    {
        private readonly ITeamHeadToHeadApi teamHeadToHeadApi;
        private readonly SportSettings soccerSettings;
        private readonly ILogger logger;

        public TeamHeadToHeadService(ISportRadarSettings sportRadarSettings, ITeamHeadToHeadApi teamHeadToHeadApi, ILogger logger)
        {
            this.teamHeadToHeadApi = teamHeadToHeadApi;
            soccerSettings = sportRadarSettings.Sports.FirstOrDefault(s => s.Id == Sport.Soccer.Value);
            this.logger = logger;
        }

        public async Task<IReadOnlyList<TeamHeadToHead>> GetTeamHeadToHeads(string homeTeamId, string awayTeamId, Language language)
        {
            IReadOnlyList<TeamHeadToHead> teamHeadToHeads = null;
            var sportRadarLanguage = language.ToSportRadarFormat();

            foreach (var region in soccerSettings.Regions)
            {
                try
                {
                    var headToHeadDto = await teamHeadToHeadApi.GetHeadToHead(
                         soccerSettings.AccessLevel,
                         soccerSettings.Version,
                         region.Name,
                         sportRadarLanguage,
                         homeTeamId,
                         awayTeamId,
                         region.Key);

                    teamHeadToHeads = TeamHeadToHeadMapper.MapHeadToHeads(headToHeadDto, region.Name);
                }
                catch (Exception ex)
                {
                    await logger.ErrorAsync(ex.Message, ex);
                }
            }

            return teamHeadToHeads;
        }
    }
}