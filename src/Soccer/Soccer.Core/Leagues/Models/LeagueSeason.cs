using System;
using MessagePack;
using Newtonsoft.Json;
using Score247.Shared.Base;

namespace Soccer.Core.Leagues.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class LeagueSeason : BaseModel
    {
        [JsonConstructor, SerializationConstructor]
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

#pragma warning disable S109 // Magic numbers should not be used
        public DateTimeOffset StartDate { get; }

        public DateTimeOffset EndDate { get; }

        public string Year { get; }

        public string LeagueId { get; }

#pragma warning restore S109 // Magic numbers should not be used
    }
}