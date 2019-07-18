namespace Soccer.DataProviders.SportRadar.Matches.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Refit;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Domain.Matches.Models;
    using Soccer.DataProviders.Matches.Services;
    using Soccer.DataProviders.SportRadar._Shared.Configurations;
    using Soccer.DataProviders.SportRadar._Shared.Enumerations;
    using Soccer.DataProviders.SportRadar._Shared.Extensions;
    using Soccer.DataProviders.SportRadar.Matches.DataMappers;
    using Soccer.DataProviders.SportRadar.Matches.Dtos;

    public interface IMatchApi
    {
        [Get("/soccer-t3/{region}/{language}/schedules/{date}/schedule.json?api_key={apiKey}")]
        Task<MatchSchedule> GetSchedule(string region, string language, string date, string apiKey);
    }

    public class MatchService : IMatchService
    {
        private readonly ISportRadarSettings sportRadarSettings;
        private readonly IMatchApi matchApi;

        public MatchService(ISportRadarSettings sportRadarSettings, IMatchApi matchApi)
        {
            this.sportRadarSettings = sportRadarSettings;
            this.matchApi = matchApi;
        }

        public async Task<IEnumerable<Match>> GetSchedule(DateTime utcFrom, DateTime utcTo, string language)
        {
            var matches = new List<Match>();
            var soccerSettings = sportRadarSettings.Sports.FirstOrDefault(s => s.Id.ToString() == Sport.Soccer.Value);
            var sportRadarLanguage = Enumeration.FromValue<Language>(language).DisplayName;

            foreach (var region in soccerSettings.Regions)
            {
                try
                {
                    var matchSchedule = await matchApi.GetSchedule(region.Name, sportRadarLanguage, DateTime.UtcNow.ToSportRadarFormat(), region.Key);

                    if (matchSchedule?.sport_events?.Any() == true)
                    {
                        matches.AddRange(matchSchedule.sport_events.Select(ms => MatchMapper.MapMatch(ms, null, region.Name)));
                    }
                }
                catch (Exception ex)
                {
                    // Refit will throw exception when 404 Not Found, so add try/catch here
                }
            }

            return matches;
        }
    }
}