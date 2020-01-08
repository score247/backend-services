using System.Linq;
using Soccer.Core.Leagues.Models;

namespace Soccer.Database.Leagues.Commands
{
    public class UpdateLeagueGroupHasStandingCommand : BaseCommand
    {
        public UpdateLeagueGroupHasStandingCommand(
            string leagueId,
            string seasonId,
            LeagueTable standings,
            string language)
        {
            LeagueId = leagueId;
            SeasonId = seasonId;
            LeagueGroups = ToJsonString(standings.GroupTables.Select(group => group.Name));
            Language = language;
        }

        public string LeagueId { get; }
        public string SeasonId { get; }
        public string LeagueGroups { get; }
        public string Language { get; }

        public override string GetSettingKey()
            => "League_UpdateGroupHasStanding";

        public override bool IsValid()
            => !string.IsNullOrEmpty(LeagueId)
                && !string.IsNullOrEmpty(SeasonId);
    }
}