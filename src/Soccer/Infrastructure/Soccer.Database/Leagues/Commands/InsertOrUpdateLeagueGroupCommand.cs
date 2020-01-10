using Soccer.Core.Leagues.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Database.Leagues.Commands
{
    public class InsertOrUpdateLeagueGroupCommand : BaseCommand
    {
        public InsertOrUpdateLeagueGroupCommand(LeagueGroupStage leagueGroupState, Language language)
        {
            LeagueId = leagueGroupState.LeagueId;
            LeagueSeasonId = leagueGroupState.LeagueSeasonId;
            GroupStageName = leagueGroupState.GroupStageName;
            GroupName = leagueGroupState.GroupName;
            HasStanding = leagueGroupState.HasStanding;
            GroupStageValue = ToJsonString(leagueGroupState);
            Language = language.DisplayName;
        }

        public string LeagueId { get; }

        public string LeagueSeasonId { get; }

        public string GroupStageName { get; }

        public string GroupName { get; }

        public string GroupStageValue { get; }

        public bool HasStanding { get; }

        public string Language { get; }

        public override string GetSettingKey() => "League_InsertGroupStage";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(LeagueId)
            && !string.IsNullOrWhiteSpace(GroupStageName);
    }
}