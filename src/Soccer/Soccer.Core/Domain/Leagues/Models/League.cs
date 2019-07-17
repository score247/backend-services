namespace Soccer.Core.Domain.Leagues.Models
{
    using Score247.Shared.Base;

    public class League : BaseEntity
    {
        public int Order { get; set; }

        public string Flag { get; set; }

        public LeagueCategory Category { get; set; }
    }
}