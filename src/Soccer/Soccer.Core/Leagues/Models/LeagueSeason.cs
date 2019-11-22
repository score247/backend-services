using System;
using MessagePack;
using Newtonsoft.Json;
using Score247.Shared.Base;

namespace Soccer.Core.Leagues.Models
{
    [MessagePackObject]
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

        [Key(2)]
        public DateTimeOffset StartDate { get; }

        [Key(3)]
        public DateTimeOffset EndDate { get; }

        [Key(4)]
        public string Year { get; }

        [Key(5)]
        public string LeagueId { get; }

#pragma warning restore S109 // Magic numbers should not be used
    }
}