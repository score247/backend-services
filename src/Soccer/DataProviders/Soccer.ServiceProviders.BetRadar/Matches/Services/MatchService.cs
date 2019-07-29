namespace Soccer.DataProviders.SportRadar.Matches.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Refit;
    using Score247.Shared.Enumerations;
    using Soccer.Core._Shared.Enumerations;
    using Soccer.Core.Matches.Models;
    using Soccer.DataProviders.Matches.Services;
    using Soccer.DataProviders.SportRadar._Shared.Configurations;
    using Soccer.DataProviders.SportRadar._Shared.Extensions;
    using Soccer.DataProviders.SportRadar.Matches.DataMappers;
    using Soccer.DataProviders.SportRadar.Matches.Dtos;

    public interface IMatchApi
    {
        [Get("/soccer-t3/{region}/{language}/schedules/{date}/schedule.json?api_key={apiKey}")]
        Task<MatchScheduleDto> GetSchedule(string region, string language, string date, string apiKey);

        [Get("/soccer-t3/{region}/{language}/schedules/{date}/results.json?api_key={apiKey}")]
        Task<Dtos.MatchResultDto> GetResult(string region, string language, string date, string apiKey);

        [Get("/soccer-t3/{region}/{language}/schedules/live/results.json?api_key={apiKey}")]
        Task<Dtos.MatchResultDto> GetLiveResult(string region, string language, string apiKey);
    }

    public class MatchService : IMatchService
    {
        private readonly IMatchApi matchApi;
        private readonly SportSettings soccerSettings;
        private readonly ILogger logger;

        public MatchService(ISportRadarSettings sportRadarSettings, IMatchApi matchApi, ILogger logger)
        {
            this.matchApi = matchApi;
            soccerSettings = sportRadarSettings.Sports.FirstOrDefault(s => s.Id == Sport.Soccer.Value);
            this.logger = logger;
        }

        public async Task<IList<Match>> GetPostMatches(DateTime utcFrom, DateTime utcTo, Language language)
        {
            var matches = new List<Match>();
            var sportRadarLanguage = language.ToSportRadarFormat();

            foreach (var region in soccerSettings.Regions)
            {
                for (var date = utcFrom.Date; date.Date <= utcTo.Date; date = date.AddDays(1))
                {
                    try
                    {
                        var matchResult = await matchApi.GetResult(region.Name, sportRadarLanguage, date.ToSportRadarFormat(), region.Key);

                        if (matchResult?.results?.Any() == true)
                        {
                            matches.AddRange(matchResult.results.Select(mr => MatchMapper.MapMatch(mr.sport_event, mr.sport_event_status, region.Name)));
                        }
                    }
                    catch (Exception ex)
                    {
                        await logger.InfoAsync(ex.Message);
                    }
                }
            }

            return matches;
        }

        public async Task<IReadOnlyList<Match>> GetPreMatches(DateTime utcFrom, DateTime utcTo, Language language)
        {
            var matches = new List<Match>();
            var sportRadarLanguage = language.ToSportRadarFormat();

            foreach (var region in soccerSettings.Regions)
            {
                for (var date = utcFrom.Date; date.Date <= utcTo.Date; date = date.AddDays(1))
                {
                    try
                    {
                        var matchSchedule = await matchApi.GetSchedule(region.Name, sportRadarLanguage, date.ToSportRadarFormat(), region.Key);

                        if (matchSchedule?.sport_events?.Any() == true)
                        {
                            matches.AddRange(matchSchedule.sport_events.Select(ms => MatchMapper.MapMatch(ms, null, region.Name)));
                        }
                    }
                    catch (Exception ex)
                    {
                        await logger.InfoAsync(ex.Message);
                    }
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
                    var liveResult = await matchApi.GetLiveResult(region.Name, sportRadarLanguage, region.Key);

                    if (liveResult?.results?.Any() == true)
                    {
                        matches.AddRange(liveResult.results.Select(mr => MatchMapper.MapMatch(mr.sport_event, mr.sport_event_status, region.Name)));
                    }
                }
                catch (Exception ex)
                {
                    await logger.InfoAsync(ex.Message);
                }
            }

            return matches;
        }
    }
}