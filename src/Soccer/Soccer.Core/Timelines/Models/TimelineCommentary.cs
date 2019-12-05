using System.Collections.Generic;
using MessagePack;
using Soccer.Core.Matches.Models;

namespace Soccer.Core.Timelines.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class TimelineCommentary
    {
        public TimelineCommentary(long timelineId, IReadOnlyList<Commentary> commentaries)
        {
            TimelineId = timelineId;
            Commentaries = commentaries;
        }

        public long TimelineId { get; }

        public IReadOnlyList<Commentary> Commentaries { get; }
    }
}