using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Logging;
using Refit;
using Score247.Shared.Enumerations;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders.SportRadar.Shared.Configurations;
using Soccer.DataProviders.SportRadar.Shared.Extensions;
using Soccer.DataProviders.SportRadar.Teams.DataMappers;
using Soccer.DataProviders.SportRadar.Teams.Dtos;
using Soccer.DataProviders.Teams.Services;

namespace Soccer.DataProviders.SportRadar.Teams.Services
{
    public interface IHeadToHeadApi
    {
        [Get("/soccer-{accessLevel}{version}/{region}/{language}/teams/{homeTeamId}/versus/{awayTeamId}/matches.json?api_key={apiKey}")]
        Task<HeadToHeadsDto> GetHeadToHead(string accessLevel, string version, string region, string language, string homeTeamId, string awayTeamId, string apiKey);

        [Get("/soccer-{accessLevel}{version}/{region}/{language}/teams/{teamId}/results.json?api_key={apiKey}")]
        Task<TeamResults> GetTeamResults(string accessLevel, string version, string region, string language, string teamId, string apiKey);
    }

    public class HeadToHeadService : IHeadToHeadService
    {
        private readonly IHeadToHeadApi headToHeadApi;
        private readonly SportSettings soccerSettings;
        private readonly ILogger logger;

        public HeadToHeadService(ISportRadarSettings sportRadarSettings, IHeadToHeadApi headToHeadApi, ILogger logger)
        {
            this.headToHeadApi = headToHeadApi;
            soccerSettings = sportRadarSettings.Sports.FirstOrDefault(s => s.Id == Sport.Soccer.Value);
            this.logger = logger;
        }

        public async Task<IReadOnlyList<Match>> GetTeamResults(string teamId, Language language)
        {
            IReadOnlyList<Match> teamResults = null;
            var sportRadarLanguage = language.ToSportRadarFormat();

            foreach (var region in soccerSettings.Regions)
            {
                try
                {
                    var teamResultsDto = await headToHeadApi.GetTeamResults(
                         soccerSettings.AccessLevel,
                         soccerSettings.Version,
                         region.Name,
                         sportRadarLanguage,
                         teamId,
                         region.Key);

                    teamResults = HeadToHeadMapper.MapTeamResults(teamResultsDto.results, region.Name);
                }
                catch (Exception ex)
                {
                    await logger.ErrorAsync(ex.Message, ex);
                }
            }

            return teamResults;
        }

        public async Task<IReadOnlyList<Match>> GetTeamHeadToHeads(string homeTeamId, string awayTeamId, Language language)
        {
            IReadOnlyList<Match> teamHeadToHeads = null;
            var sportRadarLanguage = language.ToSportRadarFormat();

            foreach (var region in soccerSettings.Regions)
            {
                try
                {
                    var headToHeadDto = await headToHeadApi.GetHeadToHead(
                         soccerSettings.AccessLevel,
                         soccerSettings.Version,
                         region.Name,
                         sportRadarLanguage,
                         homeTeamId,
                         awayTeamId,
                         region.Key);

                    teamHeadToHeads = HeadToHeadMapper.MapHeadToHeads(headToHeadDto, region.Name);
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