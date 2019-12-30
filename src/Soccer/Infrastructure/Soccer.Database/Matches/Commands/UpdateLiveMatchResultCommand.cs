namespace Soccer.Database.Matches.Commands
{
    using System;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Models;
    using Soccer.Database._Shared.Extensions;

    public class UpdateLiveMatchResultCommand : BaseCommand
    {
        public UpdateLiveMatchResultCommand(
            string matchId,
            MatchResult matchResult,
            DateTimeOffset eventDate = default)
            : base(eventDate)
        {
            SportId = Sport.Soccer.Value;
            MatchId = matchId;
            MatchResult = ToJsonString(matchResult);
        }

        public byte SportId { get; }

        public string MatchId { get; }

        public string MatchResult { get; }

        public override string GetSettingKey() => "LiveMatch_UpdateMatchResult".GetCorrespondingKey(EventDate, DateTimeOffset.Now);

        public override bool IsValid() =>
            !string.IsNullOrWhiteSpace(MatchId)
            && !string.IsNullOrWhiteSpace(MatchResult);
    }
}