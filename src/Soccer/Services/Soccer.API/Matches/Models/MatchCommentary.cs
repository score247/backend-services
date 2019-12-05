using System;
using System.Collections.Generic;
using MessagePack;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;

namespace Soccer.API.Matches.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
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
            MatchTime = timelineEvent.MatchTime;
            StoppageTime = timelineEvent.StoppageTime;
            Commentaries = timelineEvent.Commentaries;
            GoalScorer = timelineEvent.GoalScorer;
            IsPenaltyShootOutScored = (timelineEvent.IsHome && timelineEvent.IsHomeShootoutScored)
                                     || (!timelineEvent.IsHome && timelineEvent.IsAwayShootoutScored);
        }

        public string TimelineId { get; }

        public EventType TimelineType { get; }

#pragma warning disable S109 // Magic numbers should not be used

        public DateTimeOffset Time { get; }

        public byte MatchTime { get; }

        public string StoppageTime { get; }

        public IEnumerable<Commentary> Commentaries { get; }

        public GoalScorer GoalScorer { get; }

        public bool IsPenaltyShootOutScored { get; }

#pragma warning restore S109 // Magic numbers should not be used
    }
}