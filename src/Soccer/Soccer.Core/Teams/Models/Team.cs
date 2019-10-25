using MessagePack;
using Newtonsoft.Json;
using Score247.Shared.Base;

namespace Soccer.Core.Teams.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class Team : BaseModel
    {
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

        public string Country { get; private set; }

        public string CountryCode { get; private set; }

        public string Flag { get; private set; }

        public bool IsHome { get; private set; }

        public TeamStatistic Statistic { get; private set; }

        public string Abbreviation { get; private set; }

        public void SetStatistics(TeamStatistic statistic) => Statistic = statistic;
    }
}