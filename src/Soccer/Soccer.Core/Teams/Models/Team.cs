using Newtonsoft.Json;
using MessagePack;
    using System.Linq;
    using MessagePack;
using Score247.Shared.Base;

namespace Soccer.Core.Teams.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class Team : BaseModel
    {
    private const char formationSplitChar = '-';

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
        [Key(5)]

        public string Country { get; }
        [Key(6)]

        public string CountryCode { get; }
        public TeamStatistic Statistic { get; set; }

        public string Flag { get; }
        [Key(8)]

        public bool IsHome { get; }
        [Key(9)]

        public TeamStatistic Statistic { get; set; }
        [Key(10)]

        public string Abbreviation { get; }
        [Key(11)]

        [IgnoreMember]
        public IEnumerable<byte> FormationToArray => (Formation ?? string.Empty)
            .Split(formationSplitChar)
            .Where(fm => !string.IsNullOrWhiteSpace(fm))
            .Select(fm => byte.Parse(fm));

        [IgnoreMember]
        public byte TotalFormationLine => (byte)(1 + FormationToArray.Count());
    }
}