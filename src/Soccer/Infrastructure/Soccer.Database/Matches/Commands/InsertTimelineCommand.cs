namespace Soccer.Database.Matches.Commands
{
    using Soccer.Core.Matches.Models;

    public class InsertTimelineCommand : BaseCommand
    {
        public InsertTimelineCommand(
            string matchId,
            TimelineEvent timeline)

        {
            MatchId = matchId;
            Timeline = ToJsonString(timeline);
        }

        public string MatchId { get; }

        public string Timeline { get; }

        public override string GetSettingKey() => "Match_InsertTimeline";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(MatchId) && !string.IsNullOrWhiteSpace(Timeline);
    }
}