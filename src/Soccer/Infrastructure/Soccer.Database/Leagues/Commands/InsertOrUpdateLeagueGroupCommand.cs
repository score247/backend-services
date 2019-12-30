using Soccer.Core.Leagues.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Database.Leagues.Commands
{
    public class InsertOrUpdateLeagueGroupCommand : BaseCommand
    {
        public InsertOrUpdateLeagueGroupCommand(LeagueGroupState leagueGroupState, Language language)
        {
            LeagueId = leagueGroupState.LeagueId;
            LeagueSeasonId = leagueGroupState.LeagueSeasonId;
            GroupStageName = leagueGroupState.GroupStageName;
            GroupStageValue = ToJsonString(leagueGroupState);
            Language = language.DisplayName;
        }

        public string LeagueId { get; }

        public string LeagueSeasonId { get; }

        public string GroupStageName { get; }

        public string GroupStageValue { get; }

        public string Language { get; }

        public override string GetSettingKey() => "League_InsertGroupStage";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(LeagueId)
            && !string.IsNullOrWhiteSpace(GroupStageName);
    }
}