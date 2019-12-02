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
            Timelines = ToJsonString(timelines.Select(timeline => timeline.Id));
        }

        public string MatchId { get; }

        public string Timelines { get; }

        public override string GetSettingKey() => "Match_RemoveTimelines";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(MatchId) && !string.IsNullOrWhiteSpace(Timelines);
    }
}
