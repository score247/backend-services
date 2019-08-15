namespace Soccer.Database.Matches.Commands
{
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Models;

    public class UpdateLiveMatchLastTimelineCommand : BaseCommand
    {
        public UpdateLiveMatchLastTimelineCommand(string matchId, TimelineEvent timeline)
        {
            SportId = Sport.Soccer.Value;
            MatchId = matchId;
            TimelineEvent = ToJsonString(timeline);
        }

        public byte SportId { get; }

        public string MatchId { get; }

        public string TimelineEvent { get; }

        public override string GetSettingKey() => "LiveMatch_UpdateLastTimeline";

        public override bool IsValid() =>
            !string.IsNullOrWhiteSpace(MatchId)
            && !string.IsNullOrWhiteSpace(TimelineEvent);
    }
}