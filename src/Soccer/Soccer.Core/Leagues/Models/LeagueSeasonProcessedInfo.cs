using System;
using MessagePack;
using Newtonsoft.Json;

namespace Soccer.Core.Leagues.Models
{
    [MessagePackObject]
    public class LeagueSeasonProcessedInfo
    {
        [SerializationConstructor, JsonConstructor]
        public LeagueSeasonProcessedInfo(string leagueId, string seasonId, string region, sbyte fetched, DateTime fetchedDate) 
        {
            LeagueId = leagueId;
            SeasonId = seasonId;
            Region = region;
            Fetched = Convert.ToBoolean(fetched);
            FetchedDate = fetchedDate;
        }
#pragma warning disable S109 // Magic numbers should not be used
        [Key(0)]
        public string LeagueId { get; }

        [Key(1)]
        public string SeasonId { get; }

        [Key(2)]
        public string Region { get; }

        [Key(3)]
        public bool Fetched { get; }

        [Key(4)]
        public DateTime FetchedDate { get; }
#pragma warning restore S109 // Magic numbers should not be used
    }
}
