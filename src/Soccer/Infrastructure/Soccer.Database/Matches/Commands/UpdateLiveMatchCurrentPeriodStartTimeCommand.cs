namespace Soccer.Database.Matches.Commands
{
    using System;
    using Score247.Shared.Enumerations;

    public class UpdateLiveMatchCurrentPeriodStartTimeCommand : BaseCommand
    {
        public UpdateLiveMatchCurrentPeriodStartTimeCommand(string matchId, DateTimeOffset currentPeriodStartTime)
        {
            SportId = Sport.Soccer.Value;
            MatchId = matchId;
            CurrentPeriodStartTime = ToJsonString(currentPeriodStartTime);
        }

        public byte SportId { get; }

        public string MatchId { get; }

        public string CurrentPeriodStartTime { get; }

        public override string GetSettingKey() => "LiveMatch_UpdateCurrentPeriodStartTime";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(MatchId) && !string.IsNullOrWhiteSpace(CurrentPeriodStartTime);
    }
}