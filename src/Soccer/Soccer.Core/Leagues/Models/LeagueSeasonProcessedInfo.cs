using System;

namespace Soccer.Core.Leagues.Models
{
    public class LeagueSeasonProcessedInfo
    {
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
