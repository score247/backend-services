namespace Soccer.Core.Domain.Teams.Models
{
    using Score247.Shared.Base;

    public class Player : BaseEntity
    {
        public string Type { get; set; }

        public int JerseyNumber { get; set; }

        public string Position { get; set; }

        public int Order { get; set; }
    }

    public class GoalScorer : BaseEntity
    {
        public string Method { get; set; }
    }
}