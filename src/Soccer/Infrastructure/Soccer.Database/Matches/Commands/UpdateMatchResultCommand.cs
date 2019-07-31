namespace Soccer.Database.Matches.Commands
{
    using System;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Models;

    public class UpdateMatchResultCommand : BaseCommand
    {
        public UpdateMatchResultCommand(
            string matchId,
            MatchResult result,
            string language)
        {
            SportId = Sport.Soccer.Value;
            MatchId = matchId;
            MatchResult = ToJsonString(result);
            Language = language;
        }

        public byte SportId { get; }

        public string MatchId { get; }

        public string Language { get; }

        public string MatchResult { get; }

        public override string GetSettingKey() => "Score247_UpdateMatchResult";

        public override bool IsValid() =>
            !string.IsNullOrWhiteSpace(MatchId)
            && !string.IsNullOrWhiteSpace(Language)
            && !string.IsNullOrWhiteSpace(MatchResult);
    }
}