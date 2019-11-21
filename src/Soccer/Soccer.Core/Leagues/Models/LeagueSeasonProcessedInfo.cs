using System;
using MessagePack;
using Newtonsoft.Json;

namespace Soccer.Core.Leagues.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
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

        public string LeagueId { get; }

        public string SeasonId { get; }

        public string Region { get; }

        public bool Fetched { get; }

        public DateTime FetchedDate { get; }
    }
}
