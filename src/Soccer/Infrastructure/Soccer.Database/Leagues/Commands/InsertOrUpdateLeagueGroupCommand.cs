using Soccer.Core.Shared.Enumerations;

namespace Soccer.Database.Leagues.Commands
{
    public class InsertOrUpdateLeagueGroupCommand : BaseCommand
    {
        public InsertOrUpdateLeagueGroupCommand(string leagueId, string leagueGroupName, string groupName, Language language)
        {
            LeagueId = leagueId;
            LeagueGroupName = leagueGroupName;
            GroupName = groupName;
            Language = language.DisplayName;
        }

        public string LeagueId { get; }

        public string LeagueGroupName { get; }

        public string GroupName { get; }

        public string Language { get; }

        public override string GetSettingKey() => "League_InsertGroupStage";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(LeagueId)
            && !string.IsNullOrWhiteSpace(GroupName)
            && !string.IsNullOrWhiteSpace(LeagueGroupName);
    }
}