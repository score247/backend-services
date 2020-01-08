using MessagePack;
using Newtonsoft.Json;

namespace Soccer.Core.Leagues.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class LeagueGroupState
    {
        [SerializationConstructor, JsonConstructor]
        public LeagueGroupState(
            string leagueId,
            string leagueSeasonId,
            string groupStageName,
            LeagueRound leagueRound,
            bool hasStanding)
        {
            LeagueId = leagueId;
            LeagueSeasonId = leagueSeasonId;
            GroupStageName = groupStageName;
            LeagueRound = leagueRound;
            HasStanding = hasStanding;
        }

        public string LeagueId { get; }

        public string LeagueSeasonId { get; }

        public string GroupStageName { get; }

        public LeagueRound LeagueRound { get; }

        public bool HasStanding { get; }
    }
}