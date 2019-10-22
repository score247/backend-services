using Newtonsoft.Json;
using MessagePack;
using Score247.Shared.Base;

namespace Soccer.Core.Teams.Models
{
#pragma warning disable S109 // Magic numbers should not be used

    [MessagePackObject]
    public class Player : BaseModel
    {
        public Player(string id, string name) : base(id, name)
        {
        }

        [SerializationConstructor, JsonConstructor]
        public Player(string id, string name, string type, int jerseyNumber, string position, int order) : base(id, name)
        {
            Type = type;
            JerseyNumber = jerseyNumber;
            Position = position;
            Order = order;
        }

        [Key(2)]
        public PlayerType Type { get; set; }

        [Key(3)]
        public int JerseyNumber { get; }

        [Key(4)]
        public Position Position { get; set; }

        [Key(5)]
        public int Order { get; }
    }

    [MessagePackObject]
    public class GoalScorer : BaseModel
    {
        [SerializationConstructor, JsonConstructor]
        public GoalScorer(string id, string name, string method) : base(id, name)
        {
            Method = method;
        }

        [Key(2)]
        public string Method { get; }
    }

#pragma warning restore S109 // Magic numbers should not be used
}