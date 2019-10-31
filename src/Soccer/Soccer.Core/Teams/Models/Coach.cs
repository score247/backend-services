using MessagePack;
using Newtonsoft.Json;
using Score247.Shared.Base;

namespace Soccer.Core.Teams.Models
{
    [MessagePackObject]
    public class Coach : BaseModel
    {
        [JsonConstructor]
        public Coach(string id, string name, string nationality, string countryCode) : base(id, name)
        {
            Nationality = nationality;
            CountryCode = countryCode;
        }

#pragma warning disable S109 // Magic numbers should not be used

        [Key(2)]
        public string Nationality { get; }

        [Key(3)]
        public string CountryCode { get; }

#pragma warning restore S109 // Magic numbers should not be used
    }
}