using System;
using System.Collections.Generic;
using Score247.Shared.Enumerations;
using Soccer.Core.Teams.Models;

namespace Soccer.Database.Teams.Commands
{
    public class InsertOrUpdateTeamCommand : BaseCommand
    {
        public InsertOrUpdateTeamCommand(IList<TeamProfile> teams, string language)
        {
            SportId = Sport.Soccer.Value;
            Language = language;
            Teams = ToJsonString(teams);
        }

        public byte SportId { get; }

        public string Language { get; }

        public string Teams { get; }

        public override string GetSettingKey() => "Team_InsertOrUpdate";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Teams) && !string.IsNullOrWhiteSpace(Language);
    }
}
