using MessagePack;
using Newtonsoft.Json;
using Score247.Shared.Base;

namespace Soccer.Core.Teams.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class Team : BaseModel
    {
        public Team(string id, string name, bool isHome = false, TeamStatistic teamStatistic = null) : base(id, name)
        {
            IsHome = isHome;

            Statistic = teamStatistic;
        }

#pragma warning disable S107 // Methods should not have too many parameters

        [SerializationConstructor, JsonConstructor]
        public Team(
            string id,
            string name,
            string country,
            string countryCode,
            string flag,
            bool isHome,
            TeamStatistic statistic,
            string abbreviation) : base(id, name)
        {
            Country = country;
            CountryCode = countryCode;
            Flag = flag;
            IsHome = isHome;
            Statistic = statistic;
            Abbreviation = abbreviation;
        }

#pragma warning restore S107 // Methods should not have too many parameters

        public string Country { get; }

        public string CountryCode { get; }

        public string Flag { get; }

        public bool IsHome { get; }

        public TeamStatistic Statistic { get; set; }

        public string Abbreviation { get; }
    }
}