using MessagePack;
using Newtonsoft.Json;
using Score247.Shared.Base;

namespace Soccer.Core.Matches.Models
{
#pragma warning disable S109 // Magic numbers should not be used

    [MessagePackObject(keyAsPropertyName: true)]
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

        public int Capacity { get; }

        public string CityName { get; }

        public string CountryName { get; }

        public string CountryCode { get; }
    }

#pragma warning restore S109 // Magic numbers should not be used
}