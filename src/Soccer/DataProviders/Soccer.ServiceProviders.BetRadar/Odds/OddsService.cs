namespace Soccer.DataProviders.SportRadar.Odds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Refit;
    using Soccer.Core.Odds.Models;
    using Soccer.DataProviders.Odds;
    using Soccer.DataProviders.SportRadar.Odds.DataMappers;
    using Soccer.DataProviders.SportRadar.Odds.Dtos;
    using Soccer.DataProviders.SportRadar.Shared.Configurations;
    using Soccer.DataProviders.SportRadar.Shared.Extensions;

    public interface IOddsApi
    {
        [Get("/oddscomparison-rowt1/{language}/{oddsType}/sports/sr:sport:1/{date}/schedule.json?api_key={apiKey}")]
        Task<OddsScheduleDto> GetOddsSchedule(string date, string apiKey, string language = "en", string oddsType = "eu");

        [Get("/oddscomparison-rowt1/{language}/{oddsType}/sport_events/{unixTimeStamp}/changelog.json?api_key={apiKey}")]
        Task<OddsScheduleDto> GetOddsChangeLog(long unixTimeStamp, string apiKey, string language = "en", string oddsType = "eu");

        [Get("/oddscomparison-rowt1/{language}/{oddsType}/sport_events/{matchId}/markets.json?api_key={apiKey}")]
        Task<OddsScheduleDto> GetOddsByMatch(string matchId, string apiKey, string language = "en", string oddsType = "eu");
    }

    public class OddsService : IOddsService
    {
        private readonly IOddsApi oddsApi;
        private readonly OddsSetting oddsSetting;
        private readonly Func<DateTimeOffset> getCurrentTimeFunc;
        private const int dayIncrementIndex = 1;

        public OddsService(
            IOddsApi oddsApi,
            ISportRadarSettings sportRadarSettings,
            Func<DateTimeOffset> getCurrentTimeFunc)
        {
            this.oddsApi = oddsApi;
            this.oddsSetting = sportRadarSettings.SoccerSettings.OddsSetting;
            this.getCurrentTimeFunc = getCurrentTimeFunc;
        }

        public async Task<IEnumerable<MatchOdds>> GetOdds()
        {
            var from = getCurrentTimeFunc().ToUniversalTime();
            var to = from.AddDays(oddsSetting.FetchScheduleDateSpan);
            var matchOddsList = new List<MatchOdds>();

            for (var date = from.Date; date.Date <= to.Date; date = date.AddDays(dayIncrementIndex))
            {
                var oddsScheduleDto = await oddsApi.GetOddsSchedule(date.ToSportRadarFormat(), oddsSetting.Key);

                matchOddsList.AddRange(BuildMatchOddsList(oddsScheduleDto));
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
            var oddsChangeDto = await oddsApi.GetOddsChangeLog(unixTimeStamp, oddsSetting.Key);

            return BuildMatchOddsList(oddsChangeDto);
        }
    }
}