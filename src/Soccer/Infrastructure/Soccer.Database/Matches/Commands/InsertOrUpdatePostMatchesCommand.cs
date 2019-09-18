namespace Soccer.Database.Matches.Commands
{
    using System.Collections.Generic;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Models;

    public class InsertOrUpdatePostMatchesCommand : BaseCommand
    {
        public InsertOrUpdatePostMatchesCommand(
            IEnumerable<Match> matches,
            string language)

        {
            SportId = Sport.Soccer.Value;
            Matches = ToJsonString(matches);
            Language = language;
        }

        public byte SportId { get; }

        public string Matches { get; }

        public string Language { get; }

        public override string GetSettingKey() => "Match_InsertOrUpdatePostMatches";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Matches) && !string.IsNullOrWhiteSpace(Language);
    }
}