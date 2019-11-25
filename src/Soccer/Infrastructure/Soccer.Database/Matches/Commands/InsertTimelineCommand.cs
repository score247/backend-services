namespace Soccer.Database.Matches.Commands
{
    using System;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Database._Shared.Extensions;

    public class InsertTimelineCommand : BaseCommand
    {
        private const string SpName = "Match_InsertTimeline";

        public InsertTimelineCommand(
            string matchId,
            TimelineEvent timeline,
            Language language,
            DateTime eventDate = default)
            : base(eventDate)

        {
            MatchId = matchId;
            Timeline = ToJsonString(timeline);

            Language = language.DisplayName;
        }

        public string MatchId { get; }

        public string Timeline { get; }

        public string Language { get; }

        public override string GetSettingKey() => SpName.GetCorrespondingKey(EventDate);

        public override bool IsValid() => !string.IsNullOrWhiteSpace(MatchId) && !string.IsNullOrWhiteSpace(Timeline);
    }
}