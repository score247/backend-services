using System.Collections.Generic;
using Soccer.Core.Leagues.Models;

namespace Soccer.Database.Leagues.Commands
{
    public class InsertOrUpdateInternationalLeaguesCommand : InsertOrUpdateLeaguesCommand
    {
        public InsertOrUpdateInternationalLeaguesCommand(IEnumerable<League> leagues, string language) : base(leagues, language)
        {
        }

        public override string GetSettingKey() => "League_InsertInternationalLeagues";
    }
}