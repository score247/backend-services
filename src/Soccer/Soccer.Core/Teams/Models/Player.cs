using System.Collections.Generic;
using MessagePack;
using Newtonsoft.Json;
using Score247.Shared.Base;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core.Teams.Models
{
#pragma warning disable S109 // Magic numbers should not be used

    [MessagePackObject(keyAsPropertyName: true)]
    public class Player : BaseModel
    {
        public Player(string id, string name) : base(id, name)
        {
        }

        public Player(string id, string name, int jerseyNumber) : this(id, name)
        {
            JerseyNumber = jerseyNumber;
        }

        public Player(Player player)
            : this(player.Id, player.Name, player.JerseyNumber)
        { }

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

        public PlayerType Type { get; }

        public int JerseyNumber { get; }

        public Position Position { get; }

        public int Order { get; }

        public IDictionary<EventType, int> EventStatistic { get; set; }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class GoalScorer : BaseModel
    {
        public const string OwnGoal = "own_goal";
        public const string Penalty = "penalty";

        [SerializationConstructor, JsonConstructor]
        public GoalScorer(string id, string name, string method) : base(id, name)
        {
            Method = method;
        }

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