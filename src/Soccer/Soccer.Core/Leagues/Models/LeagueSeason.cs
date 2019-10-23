using System;
using Newtonsoft.Json;
using Score247.Shared.Base;

namespace Soccer.Core.Leagues.Models
{
    public class LeagueSeason : BaseModel
    {
        [JsonConstructor]
        public LeagueSeason(
            string id,
            string name,
            DateTimeOffset startDate,
            DateTimeOffset endDate,
            string year,
            string leagueId) : base(id, name)
        {
            StartDate = startDate;
            EndDate = endDate;
            Year = year;
            LeagueId = leagueId;
        }

        public DateTimeOffset StartDate { get; }

        public DateTimeOffset EndDate { get; }

        public string Year { get; }

        public string LeagueId { get; }
    }
}