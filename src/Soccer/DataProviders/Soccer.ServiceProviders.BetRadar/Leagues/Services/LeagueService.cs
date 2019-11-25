using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Logging;
using Refit;
using Score247.Shared.Enumerations;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders.Leagues;
using Soccer.DataProviders.SportRadar.Leagues.DataMappers;
using Soccer.DataProviders.SportRadar.Matches.DataMappers;
using Soccer.DataProviders.SportRadar.Shared.Configurations;
using Soccer.DataProviders.SportRadar.Shared.Extensions;

namespace Soccer.DataProviders.SportRadar.Leagues.Services
{
    public interface ISportRadarLeagueApi
    {
        [Get("/soccer-{accessLevel}{version}/{region}/{language}/tournaments.json?api_key={apiKey}")]
        Task<Dtos.TournamentResult> GetRegionLeagues(string accessLevel, string version, string region, string language, string apiKey);

        [Get("/soccer-{accessLevel}{version}/{region}/{language}/tournaments/{leagueId}/schedule.json?api_key={apiKey}")]
        Task<Dtos.TournamentSchedule> GetLeagueSchedule(string accessLevel, string version, string region, string language, string leagueId, string apiKey);

        [Get("/soccer-{accessLevel}{version}/{region}/{language}/tournaments/{tournamentId}/standings.json?api_key={apiKey}")]
        Task<Dtos.TournamentStandingDto> GetTournamentStandings(string accessLevel, string version, string region, string language, string tournamentId, string apiKey);

        [Get("/soccer-{accessLevel}{version}/{region}/{language}/tournaments/{tournamentId}/live_standings.json?api_key={apiKey}")]
        Task<Dtos.TournamentStandingDto> GetTournamentLiveStandings(string accessLevel, string version, string region, string language, string tournamentId, string apiKey);
    }

    public class SportRadarLeagueService : ILeagueService, ILeagueScheduleService
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

        public async Task<IEnumerable<Match>> GetLeagueMatches(string regionName, string leagueId, Language language)
        {
            try
            {
                var apiKey = soccerSettings.Regions.FirstOrDefault(x => x.Name == regionName).Key;

                var sportRadarLanguage = language.ToSportRadarFormat();
                var tournamentScheduleDto = await leagueApi.GetLeagueSchedule(
                    soccerSettings.AccessLevel,
                    soccerSettings.Version,
                    regionName,
                    sportRadarLanguage,
                    leagueId,
                    apiKey);

                return tournamentScheduleDto.sport_events.Select(ms => MatchMapper.MapMatch(ms, null, null, regionName, language));
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync(ex.ToString());
            }

            return Enumerable.Empty<Match>();
        }

        public async Task<IEnumerable<LeagueTable>> GetLeagueStandings(string leagueId, Language language, string regionName)
        {
            try
            {
                var apiKey = soccerSettings.Regions.FirstOrDefault(x => x.Name == regionName).Key;

                var sportRadarLanguage = language.ToSportRadarFormat();
                var tournamentStandingDto = await leagueApi.GetTournamentStandings(
                    soccerSettings.AccessLevel,
                    soccerSettings.Version,
                    regionName,
                    sportRadarLanguage,
                    leagueId,
                    apiKey);

                return tournamentStandingDto.standings.Select(standing =>
                    LeagueTableMapper.MapLeagueTable(tournamentStandingDto.tournament, tournamentStandingDto.season, tournamentStandingDto.notes, standing, regionName));
            }
            catch (ApiException ex)
            {
                await logger.ErrorAsync($"Url: {ex.RequestMessage.RequestUri}, {ex}");
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync(ex.ToString());
            }

            return Enumerable.Empty<LeagueTable>();
        }

        public async Task<IEnumerable<LeagueTable>> GetLeagueLiveStandings(string leagueId, Language language, string regionName)
        {
            try
            {
                var apiKey = soccerSettings.Regions.FirstOrDefault(x => x.Name == regionName).Key;

                var sportRadarLanguage = language.ToSportRadarFormat();
                var tournamentStandingDto = await leagueApi.GetTournamentLiveStandings(
                    soccerSettings.AccessLevel,
                    soccerSettings.Version,
                    regionName,
                    sportRadarLanguage,
                    leagueId,
                    apiKey);

                return tournamentStandingDto.standings.Select(standing =>
                    LeagueTableMapper.MapLeagueTable(tournamentStandingDto.tournament, tournamentStandingDto.season, tournamentStandingDto.notes, standing, regionName));
            }
            catch (ApiException ex)
            {
                await logger.ErrorAsync($"Url: {ex.RequestMessage.RequestUri}, {ex}");
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync(ex.ToString());
            }

            return Enumerable.Empty<LeagueTable>();
        }
    }
}