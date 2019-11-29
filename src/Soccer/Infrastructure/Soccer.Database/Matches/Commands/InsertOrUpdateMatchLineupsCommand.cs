using System;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Database._Shared.Extensions;

namespace Soccer.Database.Matches.Commands
{
    public class InsertOrUpdateMatchLineupsCommand : BaseCommand
    {
        private const string SpName = "Match_InsertOrUpdateLineups";

        public InsertOrUpdateMatchLineupsCommand(
            MatchLineups match, 
            Language language,
            DateTimeOffset eventDate = default) 
            : base(eventDate)
        {
            MatchId = match.Id;
            Lineups = ToJsonString(match);
            Language = language.DisplayName;
        }

        public string Lineups { get; }

        public string MatchId { get; }

        public string Language { get; }

        public override string GetSettingKey() 
            => SpName.GetCorrespondingKey(EventDate, DateTimeOffset.Now);

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(Lineups)
            && !string.IsNullOrWhiteSpace(MatchId)
            && !string.IsNullOrWhiteSpace(Language);
    }
}