using System;
using System.Collections.Generic;
using MessagePack;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;

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
            MatchTime = timelineEvent.MatchTime;
            StoppageTime = timelineEvent.StoppageTime;
            Commentaries = timelineEvent.Commentaries;
            GoalScorer = timelineEvent.GoalScorer;
            IsPenaltyShootOutScored = timelineEvent.IsAwayShootoutScored || timelineEvent.IsHomeShootoutScored;
        }

        [Key(0)]
        public string TimelineId { get; }

        [Key(1)]
        public EventType TimelineType { get; }

        [Key(2)]
        public DateTimeOffset Time { get; }

        [Key(3)]
        public byte MatchTime { get; }

        [Key(4)]
        public string StoppageTime { get; }

        [Key(5)]
        public IEnumerable<Commentary> Commentaries { get; }

        [Key(6)]
        public GoalScorer GoalScorer { get; }

        [Key(7)]
        public bool IsPenaltyShootOutScored { get; }
    }
}