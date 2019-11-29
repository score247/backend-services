namespace Soccer.Database.Matches.Commands
{
    using System;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Models;
    using Soccer.Database._Shared.Extensions;

    public class UpdateLiveMatchLastTimelineCommand : BaseCommand
    {
        public UpdateLiveMatchLastTimelineCommand(
            string matchId, 
            TimelineEvent timeline,
            DateTimeOffset eventDate = default)
            : base(eventDate)
        {
            SportId = Sport.Soccer.Value;
            MatchId = matchId;
            TimelineEvent = ToJsonString(timeline);
        }

        public byte SportId { get; }

        public string MatchId { get; }

        public string TimelineEvent { get; }

        public override string GetSettingKey() => "LiveMatch_UpdateLastTimeline".GetCorrespondingKey(EventDate, DateTimeOffset.Now);

        public override bool IsValid() =>
            !string.IsNullOrWhiteSpace(MatchId)
            && !string.IsNullOrWhiteSpace(TimelineEvent);
    }
}