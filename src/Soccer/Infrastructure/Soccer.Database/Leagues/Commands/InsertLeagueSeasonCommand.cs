using System.Collections.Generic;
using Soccer.Core.Leagues.Models;

namespace Soccer.Database.Leagues.Commands
{
    public class InsertLeagueSeasonCommand : BaseCommand
    {
        public InsertLeagueSeasonCommand(
               IEnumerable<League> leagues)
        {
            Leagues = ToJsonString(leagues);
        }

        public string Leagues { get; }

        public override string GetSettingKey() => "League_InsertLeagueSeason";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Leagues);
    }
}
