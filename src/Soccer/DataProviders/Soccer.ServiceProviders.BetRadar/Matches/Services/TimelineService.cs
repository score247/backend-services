namespace Soccer.DataProviders.SportRadar.Matches.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Refit;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.DataProviders.Matches.Services;
    using Soccer.DataProviders.SportRadar.Matches.DataMappers;
    using Soccer.DataProviders.SportRadar.Shared.Configurations;
    using Soccer.DataProviders.SportRadar.Shared.Extensions;

    public interface ITimelineApi
    {
        [Get("/soccer-{accessLevel}{version}/{region}/{language}/matches/{matchId}/timeline.json?api_key={apiKey}")]
        Task<Dtos.MatchTimelineDto> GetTimelines(string accessLevel, string version, string matchId, string region, string language, string apiKey);
    }

    public class TimelineService : ITimelineService
    {
        private readonly SportSettings soccerSettings;

        private readonly ITimelineApi timelineApi;
        private readonly ILogger logger;

        public TimelineService(ISportRadarSettings sportRadarSettings, ITimelineApi timelineApi, ILogger logger)
        {
            soccerSettings = sportRadarSettings.Sports.FirstOrDefault(s => s.Id == Sport.Soccer.Value);

            this.timelineApi = timelineApi;
            this.logger = logger;
        }

        public async Task<Match> GetTimelines(string matchId, string region, Language language)
        {
            var match = new Match { Id = matchId, Region = region };

            var sportRadarLanguage = language.ToSportRadarFormat();

            try
            {
                var apiKey = soccerSettings.Regions.FirstOrDefault(x => x.Name == region).Key;

                var timelineDto = await timelineApi.GetTimelines(soccerSettings.AccessLevel, soccerSettings.Version, matchId, region, sportRadarLanguage, apiKey);

                if (timelineDto.sport_event.competitors != null)
                {
                    match = MatchMapper.MapMatch(timelineDto.sport_event, timelineDto.sport_event_status, timelineDto.sport_event_conditions, region);

                    if (timelineDto?.timeline?.Any() == true)
                    {
                        match.TimeLines = timelineDto.timeline.Select(t => TimelineMapper.MapTimeline(t)).ToList();
                    }

                    if (timelineDto.coverage_info != null)
                    {
                        match.Coverage = CoverageMapper.MapCoverage(timelineDto.coverage_info);
                    }

                    if (timelineDto.statistics != null)
                    {
                        foreach (var team in timelineDto.statistics?.teams)
                        {
                            match.Teams.FirstOrDefault(x => x.Id == team.id).Statistic = StatisticMapper.MapStatistic(team.statistics);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync(ex.Message, ex);
            }

            return match;
        }
    }
}
