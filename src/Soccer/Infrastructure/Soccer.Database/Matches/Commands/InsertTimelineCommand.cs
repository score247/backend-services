namespace Soccer.Database.Matches.Commands
{    
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Shared.Enumerations;

    public class InsertTimelineCommand : BaseCommand
    {
        public InsertTimelineCommand(
            string matchId,
            TimelineEvent timeline,
            Language language)

        {
            MatchId = matchId;
            Timeline = ToJsonString(timeline);

            Language = language.DisplayName;
        }

        public string MatchId { get; }

        public string Timeline { get; }

        public string Language { get; }

        public override string GetSettingKey() => "Match_InsertTimeline";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(MatchId) && !string.IsNullOrWhiteSpace(Timeline);
    }
}