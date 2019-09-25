namespace Soccer.Database.Matches.Commands
{
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Models;

    public class UpdateMatchCoverageCommand : BaseCommand
    {
        public UpdateMatchCoverageCommand(string matchId, string language, Coverage coverage)
        {
            SportId = Sport.Soccer.Value;
            MatchId = matchId;          
            Language = language;
            Coverage = ToJsonString(coverage);
        }

        public byte SportId { get; }

        public string MatchId { get; }        

        public string Language { get; }

        public string Coverage { get; }

        public override string GetSettingKey() => "Match_UpdateCoverage";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(MatchId) 
            && !string.IsNullOrWhiteSpace(Language) 
            && !string.IsNullOrWhiteSpace(Coverage);
    }
}
