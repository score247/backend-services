namespace Soccer.Database.Matches.Commands
{
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Models;

    public class UpdateMatchResultAndMigrateLiveData : BaseCommand
    {
        public UpdateMatchResultAndMigrateLiveData(
            string matchId,
            MatchResult result)
        {
            SportId = Sport.Soccer.Value;
            MatchId = matchId;
            MatchResult = ToJsonString(result);
        }

        public byte SportId { get; }

        public string MatchId { get; }

        public string MatchResult { get; }

        public override string GetSettingKey() => "Match_UpdateMatchResultAndMigrateLiveData";

        public override bool IsValid() =>
            !string.IsNullOrWhiteSpace(MatchId)
            && !string.IsNullOrWhiteSpace(MatchResult);
    }
}