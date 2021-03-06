using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Logging;
using Refit;
using Score247.Shared.Enumerations;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;
using Soccer.Core.Timelines.Models;
using Soccer.DataProviders.Matches.Services;
using Soccer.DataProviders.SportRadar.Matches.DataMappers;
using Soccer.DataProviders.SportRadar.Matches.Dtos;
using Soccer.DataProviders.SportRadar.Shared.Configurations;
using Soccer.DataProviders.SportRadar.Shared.Extensions;

namespace Soccer.DataProviders.SportRadar.Matches.Services
{
    public interface ITimelineApi
    {
        [Get("/soccer-{accessLevel}{version}/{region}/{language}/matches/{matchId}/timeline.json?api_key={apiKey}")]
        Task<MatchTimelineDto> GetTimelineEvents(string accessLevel, string version, string matchId, string region, string language, string apiKey);
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

        public async Task<Tuple<Match, IEnumerable<TimelineCommentary>>> GetTimelineEvents(string matchId, string region, Language language)
        {
            var sportRadarLanguage = language.ToSportRadarFormat();

            try
            {
                var apiKey = soccerSettings.Regions.FirstOrDefault(x => x.Name == region)?.Key;

                var timelineDto = await timelineApi.GetTimelineEvents(soccerSettings.AccessLevel, soccerSettings.Version, matchId, region, sportRadarLanguage, apiKey);

                if (timelineDto.sport_event.competitors != null)
                {
                    var match = MatchMapper.MapMatch(
                          timelineDto.sport_event,
                          timelineDto.sport_event_status,
                          timelineDto.sport_event_conditions,
                          region,
                          language,
                          GetTimelineEvents(timelineDto),
                          GetCoverageInfo(timelineDto));

                    foreach (var team in match.Teams)
                    {
                        team.SetStatistics(GetStatistic(team.Id, timelineDto.statistics));
                    }

                    return new Tuple<Match, IEnumerable<TimelineCommentary>>(match, GetTimelineCommentaries(timelineDto));
                }
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync(ex.Message, ex);
            }

            return null;
        }

        private static IEnumerable<TimelineEvent> GetTimelineEvents(MatchTimelineDto timelineDto)
        {
            if (timelineDto?.timeline?.Any() == true)
            {
                return timelineDto.timeline.Select(TimelineMapper.MapTimeline).ToList();
            }

            return Enumerable.Empty<TimelineEvent>();
        }

        private static IEnumerable<TimelineCommentary> GetTimelineCommentaries(MatchTimelineDto timelineDto)
        {
            if (timelineDto?.timeline?.Any() == true)
            {
                return timelineDto.timeline
                    .Where(x => x.commentaries != null)
                    .Select(TimelineMapper.MapTimelineCommentary)
                    .ToList();
            }

            return Enumerable.Empty<TimelineCommentary>();
        }

        private Coverage GetCoverageInfo(MatchTimelineDto timelineDto)
            => CoverageMapper.MapCoverage(timelineDto.coverage_info, soccerSettings.TrackerWidgetLink);

        private static TeamStatistic GetStatistic(string teamId, StatisticsDto statistic)
        {
            var team = statistic?.teams?.FirstOrDefault(x => x.id == teamId);

            if (team == null || team.statistics == null)
            {
                return new TeamStatistic();
            }

            return StatisticMapper.MapStatistic(team.statistics);
        }
    }
}