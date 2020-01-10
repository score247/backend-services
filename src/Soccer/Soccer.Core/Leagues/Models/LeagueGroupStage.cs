using MessagePack;
using Newtonsoft.Json;

namespace Soccer.Core.Leagues.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class LeagueGroupStage
    {
        [SerializationConstructor, JsonConstructor]
        public LeagueGroupStage(
            string leagueId,
            string leagueSeasonId,
            string groupStageName,
            string groupName,
            LeagueRound leagueRound,
            bool hasStanding)
        {
            LeagueId = leagueId;
            LeagueSeasonId = leagueSeasonId;
            GroupStageName = groupStageName;
            GroupName = groupName;
            LeagueRound = leagueRound;
            HasStanding = hasStanding;
        }

        public string LeagueId { get; }

        public string LeagueSeasonId { get; }

        public string GroupStageName { get; }

        public string GroupName { get; }

        public LeagueRound LeagueRound { get; }

        public bool HasStanding { get; }

        public override bool Equals(object obj)
        {
            var groupStage = obj as LeagueGroupStage;
            if (groupStage == null)
            {
                return false;
            }

            return LeagueId == groupStage.LeagueId
                && LeagueSeasonId == groupStage.LeagueSeasonId
                && GroupName == groupStage.GroupName
                && GroupStageName == groupStage.GroupStageName;
        }

        public override int GetHashCode()
            => LeagueId.GetHashCode()
                + LeagueSeasonId.GetHashCode()
                + GroupName.GetHashCode()
                + GroupStageName.GetHashCode();
    }
}