namespace Soccer.Database.Matches.Commands
{
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Models;

    public class UpdateMatchResultCommand : BaseCommand
    {
        public UpdateMatchResultCommand(
            string matchId,        
            string language,
            MatchResult result)
        {
            SportId = Sport.Soccer.Value;
            MatchId = matchId;
            Language = language;
            MatchResult = ToJsonString(result);
        }

        public byte SportId { get; }

        public string MatchId { get; }

        public string Language { get; }

        public string MatchResult { get; }

        public override string GetSettingKey() => "Match_UpdateMatchResult";

        public override bool IsValid() =>
            !string.IsNullOrWhiteSpace(MatchId)
            && !string.IsNullOrWhiteSpace(Language)
            && !string.IsNullOrWhiteSpace(MatchResult);
    }
}
