using MessagePack;
using Newtonsoft.Json;
using Score247.Shared.Base;

namespace Soccer.Core.Teams.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class Coach : BaseModel
    {
        [JsonConstructor]
        public Coach(string id, string name, string nationality, string countryCode) : base(id, name)
        {
            Nationality = nationality;
            CountryCode = countryCode;
        }

#pragma warning disable S109 // Magic numbers should not be used
        public string Nationality { get; }

        public string CountryCode { get; }

#pragma warning restore S109 // Magic numbers should not be used
    }
}