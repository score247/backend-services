namespace Soccer.Core.Leagues.Models
{
    using Score247.Shared.Base;

    public class League : BaseModel
    {
        public int Order { get; set; }

        public string Flag { get; set; }

        public LeagueCategory Category { get; set; }
    }
}