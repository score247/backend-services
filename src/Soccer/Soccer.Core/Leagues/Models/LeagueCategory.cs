namespace Soccer.Core.Leagues.Models
{
    using Score247.Shared.Base;

    public class LeagueCategory : BaseModel
    {
        public LeagueCategory(string id, string name, string countryCode) : base(id, name)
        {
            CountryCode = countryCode;
        }

        public string CountryCode { get; }
    }
}