namespace Soccer.Core.Domain.Leagues.Models
{
    using Soccer.Core.Base;

    public class League : BaseEntity
    {
        public int Order { get; set; }

        public string Flag { get; set; }

        public LeagueCategory Category { get; set; }
    }
}