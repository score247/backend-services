namespace Soccer.Database.Matches.Commands
{
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Models;

    public class UpdateLiveMatchResultCommand : BaseCommand
    {
        public UpdateLiveMatchResultCommand(string matchId, MatchResult matchResult)
        {
            SportId = Sport.Soccer.Value;
            MatchId = matchId;
            MatchResult = ToJsonString(matchResult);
        }

        public byte SportId { get; }

        public string MatchId { get; }

        public string MatchResult { get; }

        public override string GetSettingKey() => "Score247_UpdateLiveMatchResult";

        public override bool IsValid() =>
            !string.IsNullOrWhiteSpace(MatchId)
            && !string.IsNullOrWhiteSpace(MatchResult);
    }
}