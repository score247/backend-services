namespace Soccer.Database.Leagues.Commands
{
    public class EnableHasGroupsCommand : BaseCommand
    {
        public EnableHasGroupsCommand(string leagueId)
        {
            LeagueId = leagueId;
        }

        public string LeagueId { get; }

        public override string GetSettingKey() => "League_EnableHasGroups";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(LeagueId);
    }
}