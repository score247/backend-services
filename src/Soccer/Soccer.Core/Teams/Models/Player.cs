using System.Collections.Generic;
using MessagePack;
using Newtonsoft.Json;
using Score247.Shared.Base;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core.Teams.Models
{
#pragma warning disable S109 // Magic numbers should not be used

    [MessagePackObject]
    public class Player : BaseModel
    {
        public Player(string id, string name) : base(id, name)
        {
        }

        public Player(string id, string name, int jerseyNumber) : this(id, name)
        {
            JerseyNumber = jerseyNumber;
        }

        [SerializationConstructor, JsonConstructor]
        public Player(
            string id,
            string name,
            PlayerType type,
            int jerseyNumber,
            Position position,
            int order) : base(id, name)
        {
            Type = type;
            JerseyNumber = jerseyNumber;
            Position = position;
            Order = order;
        }

        [Key(2)]
        public PlayerType Type { get; }

        [Key(3)]
        public int JerseyNumber { get; }

        [Key(4)]
        public Position Position { get; }

        [Key(5)]
        public int Order { get; }

        [Key(6)]
        public IDictionary<EventType, int> EventStatistic { get; set; }
    }

    [MessagePackObject]
    public class GoalScorer : BaseModel
    {
        public const string OwnGoal = "own_goal";
        public const string Penalty = "penalty";

        [SerializationConstructor, JsonConstructor]
        public GoalScorer(string id, string name, string method) : base(id, name)
        {
            Method = method;
        }

        [Key(2)]
        public string Method { get; }

        public EventType GetEventTypeFromGoalMethod()
            => Method == OwnGoal
                ? EventType.ScoreChangeByOwnGoal
#pragma warning disable S3358 // Ternary operators should not be nested
                : Method == Penalty
                    ? EventType.ScoreChangeByPenalty
                    : EventType.ScoreChange;
#pragma warning restore S3358 // Ternary operators should not be nested
    }

#pragma warning restore S109 // Magic numbers should not be used
}