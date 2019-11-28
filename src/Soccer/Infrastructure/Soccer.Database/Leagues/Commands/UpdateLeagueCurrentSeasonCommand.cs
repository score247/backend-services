using System.Collections.Generic;
using Soccer.Core.Leagues.Models;

namespace Soccer.Database.Leagues.Commands
{
    public class UpdateLeagueCurrentSeasonCommand : BaseCommand
    {
        public UpdateLeagueCurrentSeasonCommand(IEnumerable<League> leagues)
        {
            Leagues = ToJsonString(leagues);
        }

        public string Leagues { get; }

        public override string GetSettingKey() => "League_UpdateCurrentSeason";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Leagues);
    }
}
