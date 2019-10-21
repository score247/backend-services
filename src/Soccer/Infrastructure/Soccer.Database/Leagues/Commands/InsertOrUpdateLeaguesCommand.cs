using System.Collections.Generic;
using Score247.Shared.Enumerations;
using Soccer.Core.Leagues.Models;

namespace Soccer.Database.Leagues.Commands
{
    public class InsertOrUpdateLeaguesCommand : BaseCommand
    {
        public InsertOrUpdateLeaguesCommand(
               IEnumerable<League> leagues,
               string language)

        {
            SportId = Sport.Soccer.Value;
            Leagues = ToJsonString(leagues);
            Language = language;
        }

        public byte SportId { get; }
        public string Leagues { get; }

        public string Language { get; }

        public override string GetSettingKey() => "League_InsertLeagues";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Leagues) && !string.IsNullOrWhiteSpace(Language);
    }
}