using Newtonsoft.Json;
using MessagePack;
using Score247.Shared.Base;

namespace Soccer.Core.Matches.Models
{
#pragma warning disable S109 // Magic numbers should not be used

    [MessagePackObject]
    public class Venue : BaseModel
    {
        [SerializationConstructor, JsonConstructor]
        public Venue(string id, string name, int capacity, string cityName, string countryName, string countryCode) : base(id, name)
        {
            Capacity = capacity;
            CityName = cityName;
            CountryName = countryName;
            CountryCode = countryCode;
        }

        [Key(2)]
        public int Capacity { get; }

        [Key(3)]
        public string CityName { get; }

        [Key(4)]
        public string CountryName { get; }

        [Key(5)]
        public string CountryCode { get; }
    }

#pragma warning restore S109 // Magic numbers should not be used
}