using System.Collections.Generic;
using MessagePack;
using Soccer.Core.Matches.Models;

namespace Soccer.Core.Timeline.Models
{
    [MessagePackObject]
    public class TimelineCommentary
    {
        public TimelineCommentary(long timelineId, IReadOnlyList<Commentary> commentaries) 
        {
            TimelineId = timelineId;
            Commentaries = commentaries;
        }

        [Key(0)]
        public long TimelineId { get; }

        [Key(1)]
        public IReadOnlyList<Commentary> Commentaries { get; }
    }
}
