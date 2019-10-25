namespace Soccer.DataProviders.SportRadar.Matches.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Refit;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.DataProviders.Matches.Services;
    using Soccer.DataProviders.SportRadar.Matches.DataMappers;
    using Soccer.DataProviders.SportRadar.Matches.Dtos;
    using Soccer.DataProviders.SportRadar.Shared.Configurations;
    using Soccer.DataProviders.SportRadar.Shared.Extensions;

    public interface IMatchApi
    {
        [Get("/soccer-{accessLevel}{version}/{region}/{language}/schedules/{date}/schedule.json?api_key={apiKey}")]
        Task<MatchScheduleDto> GetSchedule(string accessLevel, string version, string region, string language, string date, string apiKey);

        [Get("/soccer-{accessLevel}{version}/{region}/{language}/schedules/{date}/results.json?api_key={apiKey}")]
        Task<Dtos.MatchResultDto> GetResult(string accessLevel, string version, string region, string language, string date, string apiKey);

        [Get("/soccer-{accessLevel}{version}/{region}/{language}/schedules/live/results.json?api_key={apiKey}")]
        Task<Dtos.MatchResultDto> GetLiveResult(string accessLevel, string version, string region, string language, string apiKey);

        [Get("/soccer-{accessLevel}{version}/{region}/{language}/matches/{matchId}/lineups.json?api_key={apiKey}")]
        Task<Dtos.MatchLineupsDto> GetLineups(string accessLevel, string version, string matchId, string region, string language, string apiKey);
    }

    public class MatchService : IMatchService
    {
        private readonly string[] IgnoreMessages = new[] { "No results", "No events" };
        private readonly IMatchApi matchApi;

        private readonly SportSettings soccerSettings;
        private readonly ILogger logger;

        public MatchService(ISportRadarSettings sportRadarSettings, IMatchApi matchApi, ILogger logger)
        {
            this.matchApi = matchApi;
            soccerSettings = sportRadarSettings.Sports.FirstOrDefault(s => s.Id == Sport.Soccer.Value);
            this.logger = logger;
        }

        public async Task<IReadOnlyList<Match>> GetPostMatches(DateTime date, Language language)
        {
            var matches = new List<Match>();
            var sportRadarLanguage = language.ToSportRadarFormat();

            foreach (var region in soccerSettings.Regions)
            {
                try
                {
                    var matchResult = await matchApi.GetResult(
                        soccerSettings.AccessLevel, soccerSettings.Version, region.Name, sportRadarLanguage, date.ToSportRadarFormat(), region.Key);

                    if (matchResult?.results?.Any() == true)
                    {
                        matches.AddRange(matchResult.results.Select(mr => MatchMapper.MapMatch(mr.sport_event, mr.sport_event_status, null, region.Name)));
                    }
                }
                catch (Exception ex)
                {
                    await LogApiException(ex);
                }
            }

            return matches;
        }

        public async Task<IReadOnlyList<Match>> GetPreMatches(DateTime date, Language language)
        {
            var matches = new List<Match>();
            var sportRadarLanguage = language.ToSportRadarFormat();

            foreach (var region in soccerSettings.Regions)
            {
                try
                {
                    var matchSchedule = await matchApi.GetSchedule(
                            soccerSettings.AccessLevel,
                            soccerSettings.Version,
                            region.Name,
                            sportRadarLanguage,
                            date.ToSportRadarFormat(),
                            region.Key);

                    if (matchSchedule?.sport_events?.Any() == true)
                    {
                        matches.AddRange(matchSchedule.sport_events.Select(ms => MatchMapper.MapMatch(ms, null, null, region.Name)));
                    }
                }
                catch (Exception ex)
                {
                    await LogApiException(ex);
                }
            }

            return matches;
        }

        public async Task<IReadOnlyList<Match>> GetLiveMatches(Language language)
        {
            var matches = new List<Match>();
            var sportRadarLanguage = language.ToSportRadarFormat();

            foreach (var region in soccerSettings.Regions)
            {
                try
                {
                    var liveResult = await matchApi.GetLiveResult(soccerSettings.AccessLevel, soccerSettings.Version, region.Name, sportRadarLanguage, region.Key);

                    if (liveResult?.results?.Any() == true)
                    {
                        matches.AddRange(liveResult.results.Select(mr => MatchMapper.MapMatch(mr.sport_event, mr.sport_event_status, null, region.Name)));
                    }
                }
                catch (Exception ex)
                {
                    await LogApiException(ex);
                }
            }

            return matches;
        }

        private async Task LogApiException(Exception ex)
        {
            var apiException = (ApiException)ex;
            var content = apiException.Content;

            if (IgnoreMessages.All(m => !content.Contains(m)))
            {
                var message = $"Response: {content} \r\nRequest URL: {apiException.RequestMessage.RequestUri}";
                await logger.ErrorAsync(message, ex);
            }
        }

        public async Task<MatchLineups> GetLineups(string matchId, string region, Language language)
        {
            try
            {
                var apiKey = soccerSettings.Regions.FirstOrDefault(x => x.Name == region).Key;
                var matchLineUp = await matchApi.GetLineups(soccerSettings.AccessLevel, soccerSettings.Version, matchId, region, language.ToSportRadarFormat(), apiKey);

                if (matchLineUp?.sport_event != null
                    && matchLineUp?.lineups != null)
                {
                    return LineupsMapper.MapLineups(matchLineUp, region);
                }
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync(ex.Message, ex);
            }

            return default(MatchLineups);
        }
    }
}