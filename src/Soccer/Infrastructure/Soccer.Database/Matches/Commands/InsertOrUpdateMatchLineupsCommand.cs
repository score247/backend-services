using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Database.Matches.Commands
{
    public class InsertOrUpdateMatchLineupsCommand : BaseCommand
    {
        public InsertOrUpdateMatchLineupsCommand(MatchLineups match, Language language)
        {
            MatchId = match.Id;
            Lineups = ToJsonString(match);
            Language = language.DisplayName;
        }

        public string Lineups { get; }

        public string MatchId { get; }

        public string Language { get; }

        public override string GetSettingKey()
            => "Match_InsertOrUpdateLineups";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(Lineups)
            && !string.IsNullOrWhiteSpace(MatchId)
            && !string.IsNullOrWhiteSpace(Language);
    }
}