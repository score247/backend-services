namespace Soccer.Database.Matches.Commands
{
    using System;
    using Soccer.Core.Matches.Models;

    public class InsertTimelineCommand : BaseCommand
    {
        public InsertTimelineCommand(
            string matchId,
            Timeline timeline)

        {
            MatchId = matchId;
            Timeline = ToJsonString(timeline);
        }

        public string MatchId { get; }

        public string Timeline { get; set; }

        public override string GetSettingKey() => "Score247_InsertTimeline";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(MatchId) && !string.IsNullOrEmpty(Timeline);
    }
}