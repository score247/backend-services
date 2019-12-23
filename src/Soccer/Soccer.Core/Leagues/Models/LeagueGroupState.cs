using MessagePack;
using Newtonsoft.Json;

namespace Soccer.Core.Leagues.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class LeagueGroupState
    {
        [SerializationConstructor, JsonConstructor]
        public LeagueGroupState(
            int id,
            string leagueId,
            string groupStageName,
            string language)
        {
            Id = id;
            LeagueId = leagueId;
            GroupStageName = groupStageName;
            Language = language;
        }

        public int Id { get; }
        public string LeagueId { get; }
        public string GroupStageName { get; }
        public string Language { get; }
    }
}