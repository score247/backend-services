using System;
using System.Collections.Generic;
using MessagePack;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.API.Matches.Models
{
    [MessagePackObject]
    public class MatchCommentary
    {
        [SerializationConstructor]
        public MatchCommentary()
        {
        }

        public MatchCommentary(TimelineEvent timelineEvent)
        {
            TimelineId = timelineEvent.Id;
            TimelineType = timelineEvent.Type;
            Time = timelineEvent.Time;
            Commentaries = timelineEvent.Commentaries;
        }

        [Key(0)]
        public string TimelineId { get; }

        [Key(1)]
        public EventType TimelineType { get; }

        [Key(2)]
        public DateTimeOffset Time { get; }

        [Key(3)]
        public IEnumerable<Commentary> Commentaries { get; }
    }
}