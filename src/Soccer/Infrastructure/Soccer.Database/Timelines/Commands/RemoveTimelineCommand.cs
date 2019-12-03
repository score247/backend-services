using System.Collections.Generic;
using System.Linq;
using Soccer.Core.Matches.Models;

namespace Soccer.Database.Timelines.Commands
{
    public class RemoveTimelineCommand : BaseCommand
    {
        public RemoveTimelineCommand(string matchId, IReadOnlyList<TimelineEvent> timelines)
        {
            MatchId = matchId;
            TimelineIds = ToJsonString(timelines.Select(timeline => new { timeline.Id }));
        }

        public string MatchId { get; }

        public string TimelineIds { get; }

        public override string GetSettingKey() => "Match_RemoveTimelines";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(MatchId) && !string.IsNullOrWhiteSpace(TimelineIds);
    }
}
