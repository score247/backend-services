namespace Soccer.Database.Matches.Commands
{
    using System;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Models;
    using Soccer.Database._Shared.Extensions;

    public class UpdateLiveMatchInjuryTimesCommand : BaseCommand
    {
        public UpdateLiveMatchInjuryTimesCommand(
            string matchId,
            InjuryTimes injuryTimes,
            DateTimeOffset eventDate = default)
            : base(eventDate)
        {
            SportId = Sport.Soccer.Value;
            MatchId = matchId;
            InjuryTimes = ToJsonString(injuryTimes);
        }

        public byte SportId { get; }

        public string MatchId { get; }

        public string InjuryTimes { get; }

        public override string GetSettingKey() => "LiveMatch_UpdateInjuryTime".GetCorrespondingKey(EventDate, DateTimeOffset.Now);

        public override bool IsValid() =>
            !string.IsNullOrWhiteSpace(MatchId)
            && !string.IsNullOrWhiteSpace(InjuryTimes);
    }
}