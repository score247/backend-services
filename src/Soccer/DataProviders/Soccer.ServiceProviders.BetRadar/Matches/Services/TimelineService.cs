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
    using Soccer.DataProviders.SportRadar.Shared.Configurations;
    using Soccer.DataProviders.SportRadar.Shared.Extensions;

    public interface ITimelineApi
    {
        [Get("/soccer-t3/{region}/{language}/matches/{matchId}/timeline.json?api_key={apiKey}")]
        Task<Dtos.MatchTimelineDto> GetTimelines(string matchId, string region, string language, string apiKey);
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
               
                var timelineDto = await timelineApi.GetTimelines(matchId, region, sportRadarLanguage, apiKey);
                match = MatchMapper.MapMatch(timelineDto.sport_event, timelineDto.sport_event_status, timelineDto.sport_event_conditions, region);             

                if (timelineDto?.timeline?.Any() == true)
                {                    
                    match.TimeLines = timelineDto.timeline.Select(t => TimelineMapper.MapTimeline(t)).ToList();
                }

                //TODO handle map coverage info
                //TODO handle map statistic
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync(ex.Message, ex);
            }

            return match;
        }
    }
}
