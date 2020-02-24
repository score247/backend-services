using MessagePack;
using Newtonsoft.Json;
using Score247.Shared.Base;

namespace Soccer.Core.Teams.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class Team : TeamProfile
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
            string abbreviation,
            string logoName = null) : base(id, name, country, countryCode, abbreviation, logoName)
        {
            Flag = flag;
            IsHome = isHome;
            Statistic = statistic;
        }

#pragma warning restore S107 // Methods should not have too many parameters

        public string Flag { get; private set; }

        public bool IsHome { get; private set; }

        public TeamStatistic Statistic { get; private set; }

        public void SetStatistics(TeamStatistic statistic) => Statistic = statistic;
    }
}