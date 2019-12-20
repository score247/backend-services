using System;
using MessagePack;
using Newtonsoft.Json;
using Score247.Shared.Base;
using Soccer.Core.Leagues.Extensions;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core.Leagues.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
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
            string region,
            string currentSeasonId) : base(id, name)
        {
            Order = order;
            CategoryId = categoryId;
            CountryName = countryName;
            CountryCode = countryCode;
            IsInternational = Convert.ToBoolean(isInternational);
            Region = region;
            SeasonId = currentSeasonId;
        }

        public League(League league, string leagueName)
            : this(
                  league.Id,
                  leagueName,
                  league.Order,
                  league.CategoryId,
                  league.CountryName,
                  league.CountryCode,
                  league.IsInternational,
                  league.Region,
                  league.SeasonId)
        {
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
            string region,
            string seasonId) : base(id, name)
        {
            Order = order;
            CategoryId = categoryId;
            CountryName = countryName;
            CountryCode = countryCode;
            IsInternational = isInternational;
            Region = region;
            SeasonId = seasonId;
        }

#pragma warning restore S107 // Methods should not have too many parameters

#pragma warning disable S109 // Magic numbers should not be used

        public int Order { get; private set; }

        public string CategoryId { get; }

        public string CountryName { get; }

        public string CountryCode { get; private set; }

        public bool IsInternational { get; private set; }

        public string Region { get; private set; }

        public string SeasonId { get; private set; }

#pragma warning restore S109 // Magic numbers should not be used

        public void UpdateLeague(string countryCode, bool isInternational, int order, string region)
        {
            CountryCode = countryCode;
            IsInternational = isInternational;
            Order = order;
            Region = region;
        }

        public void UpdateLeagueName(string leagueName)
        {
            Name = leagueName;
        }
    }
}