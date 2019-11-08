using System;
using MessagePack;
using Newtonsoft.Json;
using Score247.Shared.Base;

namespace Soccer.Core.Leagues.Models
{
    [MessagePackObject]
    public class League : BaseModel
    {
        public League(string id, string name) : base(id, name)
        {
        }

#pragma warning disable S107 // Methods should not have too many parameters

        public League(
            string id,
            string name,
            int order,
            string categoryId,
            string countryName,
            string countryCode,
            sbyte isInternational,
            string region) : base(id, name)
        {
            Order = order;
            CategoryId = categoryId;
            CountryName = countryName;
            CountryCode = countryCode;
            IsInternational = Convert.ToBoolean(isInternational);
            Region = region;
        }

        [SerializationConstructor, JsonConstructor]
#pragma warning disable S107 // Methods should not have too many parameters
        public League(
            string id,
            string name,
            int order,
            string categoryId,
            string countryName,
            string countryCode,
            bool isInternational,
            string region) : base(id, name)
        {
            Order = order;
            CategoryId = categoryId;
            CountryName = countryName;
            CountryCode = countryCode;
            IsInternational = isInternational;
            Region = region;
        }

#pragma warning restore S107 // Methods should not have too many parameters

#pragma warning disable S109 // Magic numbers should not be used

        [Key(2)]
        public int Order { get; private set; }

        [Key(3)]
        public string CategoryId { get; }

        [Key(4)]
        public string CountryName { get; }

        [Key(5)]
        public string CountryCode { get; private set; }

        [Key(6)]
        public bool IsInternational { get; private set; }

        [Key(7)]
        public string Region { get; private set; }

#pragma warning restore S109 // Magic numbers should not be used

        public void UpdateLeague(string countryCode, bool isInternational, int order, string region)
        {
            CountryCode = countryCode;
            IsInternational = isInternational;
            Order = order;
            Region = region;
        }
    }
}