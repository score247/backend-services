using Soccer.Core.Leagues.Models;

namespace Soccer.Database.Leagues.Commands
{
    public class InsertOrUpdateStandingCommand : BaseCommand
    {
        public InsertOrUpdateStandingCommand(
            string leagueId,
            string seasonId,
            string tableType,
            LeagueTable standings,
            string language)
        {
            LeagueId = leagueId;
            SeasonId = seasonId;
            TableType = tableType;
            Standings = ToJsonString(standings);
            Language = language;
        }

        public string LeagueId { get; }
        public string SeasonId { get; }
        public string TableType { get; }
        public string Standings { get; }
        public string Language { get; }

        public override string GetSettingKey()
            => "League_InsertOrUpdateStandings";

        public override bool IsValid()
            => !string.IsNullOrEmpty(LeagueId)
                && !string.IsNullOrEmpty(SeasonId)
                && !string.IsNullOrEmpty(TableType);
    }
}