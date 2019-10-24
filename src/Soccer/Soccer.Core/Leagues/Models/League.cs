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

        [JsonConstructor]
#pragma warning disable S107 // Methods should not have too many parameters
        public League(
            string id,
            string name,
            int order,
            string countryCode) : base(id, name)
        {
            Order = order;
            CountryCode = countryCode;
        }

        [SerializationConstructor]
#pragma warning disable S107 // Methods should not have too many parameters
        public League(
            string id,
            string name,
            int order,
            string categoryId,
            string countryName,
            string countryCode,
            bool isInternational,
            string region) : this(id, name, order, countryCode)
        {
            CategoryId = categoryId;
            CountryName = countryName;
            IsInternational = isInternational;
            Region = region;
        }

#pragma warning restore S107 // Methods should not have too many parameters

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

        [IgnoreMember]
        public int IsActive { get; }

        [IgnoreMember]
        public string Region { get; }

        public void SetInternationalLeagueCode(string countryCode)
        {
            CountryCode = countryCode;
            IsInternational = true;
        }

        public void SetOrder(int order)
        {
            Order = order;
        }
    }
}