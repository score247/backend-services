using System.Collections.Generic;
using Soccer.Core.Leagues.Models;

namespace Soccer.Database.Leagues.Commands
{
    public class UpdateFetchedLeagueSeasonCommand : BaseCommand
    {
        public UpdateFetchedLeagueSeasonCommand(IEnumerable<LeagueSeasonProcessedInfo> leagueSeasons)
        {
            LeagueSeasons = ToJsonString(leagueSeasons);
        }

        public string LeagueSeasons { get; }

        public override string GetSettingKey() => "League_UpdateFetchedLeagueSeason";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(LeagueSeasons);
    }
}
