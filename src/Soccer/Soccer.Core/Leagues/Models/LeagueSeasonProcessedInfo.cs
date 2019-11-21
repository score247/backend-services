using System;
using MessagePack;
using Newtonsoft.Json;

namespace Soccer.Core.Leagues.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class LeagueSeasonProcessedInfo
    {
        [SerializationConstructor, JsonConstructor]
        public LeagueSeasonProcessedInfo(string leagueId, string seasonId, string region, bool fetched, DateTimeOffset fetchedDate) 
        {
            LeagueId = leagueId;
            SeasonId = seasonId;
            Region = region;
            Fetched = fetched;
            FetchedDate = fetchedDate;
        }

        public string LeagueId { get; }

        public string SeasonId { get; }

        public string Region { get; }

        public bool Fetched { get; }

        public DateTimeOffset FetchedDate { get; }
    }
}
