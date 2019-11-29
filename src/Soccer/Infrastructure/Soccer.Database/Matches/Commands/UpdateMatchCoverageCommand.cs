namespace Soccer.Database.Matches.Commands
{
    using System;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Models;
    using Soccer.Database._Shared.Extensions;

    public class UpdateMatchCoverageCommand : BaseCommand
    {
        public UpdateMatchCoverageCommand(string matchId, Coverage coverage, DateTimeOffset eventDate) : base(eventDate)
        {
            SportId = Sport.Soccer.Value;
            MatchId = matchId;
            Coverage = ToJsonString(coverage);
        }

        public byte SportId { get; }

        public string MatchId { get; }

        public string Coverage { get; }

        public override string GetSettingKey() => "Match_UpdateCoverage".GetCorrespondingKey(EventDate, DateTimeOffset.Now);

        public override bool IsValid() => !string.IsNullOrWhiteSpace(MatchId)
            && !string.IsNullOrWhiteSpace(Coverage);
    }
}
