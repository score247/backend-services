using MessagePack;
using Newtonsoft.Json;
using Score247.Shared.Base;

namespace Soccer.Core.Teams.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class TeamProfile : BaseModel
    {
        [SerializationConstructor, JsonConstructor]
        public TeamProfile(
              string id,
              string name,
              string country,
              string countryCode,
              string abbreviation,
              string logoName) : base(id, name)
        {
            Country = country;
            CountryCode = countryCode;
            Abbreviation = abbreviation;
            LogoName = logoName;
        }

        public string Country { get; private set; }

        public string CountryCode { get; private set; }

        public string Abbreviation { get; private set; }

        public string LogoName { get; private set; }
    }
}
