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
            LeagueRound leagueRound)
        {
            LeagueId = leagueId;
            LeagueSeasonId = leagueSeasonId;
            GroupStageName = groupStageName;
            LeagueRound = leagueRound;
        }

        public string LeagueId { get; }

        public string LeagueSeasonId { get; }

        public string GroupStageName { get; }

        public LeagueRound LeagueRound { get; }
    }
}