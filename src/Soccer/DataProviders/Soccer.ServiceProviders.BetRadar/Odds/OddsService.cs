namespace Soccer.DataProviders.SportRadar.Odds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Newtonsoft.Json;
    using Refit;
    using Soccer.Core.Odds.Models;
    using Soccer.DataProviders.Odds;
    using Soccer.DataProviders.SportRadar.Odds.DataMappers;
    using Soccer.DataProviders.SportRadar.Odds.Dtos;
    using Soccer.DataProviders.SportRadar.Shared.Configurations;
    using Soccer.DataProviders.SportRadar.Shared.Extensions;

    public interface IOddsApi
    {
        [Get("/oddscomparison-row{accessLevel}{version}/{language}/{oddsType}/sports/sr:sport:1/{date}/schedule.json?api_key={apiKey}")]
        Task<OddsScheduleDto> GetOddsSchedule(string accessLevel, string version, string date, string apiKey, string language = "en", string oddsType = "eu");

        [Get("/oddscomparison-row{accessLevel}{version}/{language}/{oddsType}/sport_events/{unixTimeStamp}/changelog.json?api_key={apiKey}")]
        Task<OddsScheduleDto> GetOddsChangeLog(string accessLevel, string version, long unixTimeStamp, string apiKey, string language = "en", string oddsType = "eu");

        [Get("/oddscomparison-row{accessLevel}{version}/{language}/{oddsType}/sport_events/{matchId}/markets.json?api_key={apiKey}")]
        Task<OddsByMatchDto> GetOddsByMatch(string accessLevel, string version, string matchId, string apiKey, string language = "en", string oddsType = "eu");
    }

    public class OddsService : IOddsService
    {
        private readonly IOddsApi oddsApi;
        private readonly OddsSetting oddsSetting;
        private readonly Func<DateTimeOffset> getCurrentTimeFunc;
        private readonly ILogger logger;
        private const int dayIncrementIndex = 1;

        public OddsService(
            IOddsApi oddsApi,
            ISportRadarSettings sportRadarSettings,
            Func<DateTimeOffset> getCurrentTimeFunc,
            ILogger logger)
        {
            this.oddsApi = oddsApi;
            oddsSetting = sportRadarSettings.SoccerSettings.OddsSetting;
            this.getCurrentTimeFunc = getCurrentTimeFunc;
            this.logger = logger;
        }

        public async Task<IEnumerable<MatchOdds>> GetOdds()
        {
            var from = getCurrentTimeFunc().ToUniversalTime();
            var to = from.AddDays(oddsSetting.FetchScheduleDateSpan);
            var matchOddsList = new List<MatchOdds>();

            for (var date = from.Date; date.Date <= to.Date; date = date.AddDays(dayIncrementIndex))
            {
                try
                {
                    var oddsScheduleDto = await oddsApi.GetOddsSchedule(oddsSetting.AccessLevel, oddsSetting.Version, date.ToSportRadarFormat(), oddsSetting.Key);
                    matchOddsList.AddRange(BuildMatchOddsList(oddsScheduleDto));
                }
                catch (Exception ex)
                {
                    await logger.ErrorAsync(ex.ToString());
                }
            }

            return matchOddsList;
        }

        private static IEnumerable<MatchOdds> BuildMatchOddsList(OddsScheduleDto oddsScheduleDto)
            => oddsScheduleDto != null
                ? OddsMapper.MapToMatchOddsList(oddsScheduleDto)
                : Enumerable.Empty<MatchOdds>();

        public async Task<IEnumerable<MatchOdds>> GetOddsChange(int minuteInterval)
        {
            var unixTimeStamp = getCurrentTimeFunc().AddMinutes(-minuteInterval).ToUnixTimeSeconds();
            var oddsChangeDto = await oddsApi.GetOddsChangeLog(oddsSetting.AccessLevel, oddsSetting.Version, unixTimeStamp, oddsSetting.Key);

            return BuildMatchOddsList(oddsChangeDto);
        }

        public async Task<MatchOdds> GetOdds(string matchId, DateTime lastUpdated)
        {
            var oddsByMatchDto = await oddsApi.GetOddsByMatch(oddsSetting.AccessLevel, oddsSetting.Version, matchId, oddsSetting.Key);

            await logger.InfoAsync($"Get Odds API: {matchId} at {DateTime.Now} \r\n{JsonConvert.SerializeObject(oddsByMatchDto)}");

            var matchOdds = OddsMapper.MapToMatchOdds(
                oddsByMatchDto.sport_event
                    ?? new SportEvent
                    {
                        id = matchId,
                        markets_last_updated = lastUpdated
                    });

            matchOdds.SetLastUpdated(lastUpdated);

            return matchOdds;
        }
    }
}