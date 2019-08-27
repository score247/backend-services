namespace Soccer.Database.Matches.Commands
{
    using System;
    using Score247.Shared.Enumerations;

    public class UpdateLiveMatchCurrentPeriodStartTimeCommand : BaseCommand
    {
        public UpdateLiveMatchCurrentPeriodStartTimeCommand(string matchId, DateTime currentPeriodStartTime)
        {
            SportId = Sport.Soccer.Value;
            MatchId = matchId;
            CurrentPeriodStartTime = currentPeriodStartTime.ToUniversalTime();
        }

        public byte SportId { get; }

        public string MatchId { get; }

        public DateTime CurrentPeriodStartTime { get; }

        public override string GetSettingKey() => "LiveMatch_UpdateCurrentPeriodStartTime";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(MatchId) && CurrentPeriodStartTime != DateTime.MinValue;
    }
}