namespace Soccer.Core.Teams.Models
{
    using MessagePack;
    using Score247.Shared.Base;
#pragma warning disable S109 // Magic numbers should not be used
    [MessagePackObject]
    public class Player : BaseModel
    {
        [Key(2)]
        public string Type { get; set; }

        [Key(3)]
        public int JerseyNumber { get; set; }

        [Key(4)]
        public string Position { get; set; }

        [Key(5)]
        public int Order { get; set; }
    }

    [MessagePackObject]
    public class GoalScorer : BaseModel
    {
        [Key(2)]
        public string Method { get; set; }
    }
#pragma warning restore S109 // Magic numbers should not be used
}