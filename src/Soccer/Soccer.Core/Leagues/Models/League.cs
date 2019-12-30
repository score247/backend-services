using System;
using MessagePack;
using Newtonsoft.Json;
using Score247.Shared.Base;

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
            string currentSeasonId,
            LeagueSeasonDates seasonDates,
            long hasGroups = 0)
                : this(
                    id,
                    name,
                    order,
                    categoryId,
                    countryName,
                    countryCode,
                    Convert.ToBoolean(isInternational),
                    region,
                    currentSeasonId,
                    seasonDates,
                    Convert.ToBoolean(hasGroups))
        {
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
                  league.SeasonId,
                  league.SeasonDates,
                  league.HasGroups)
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
            string seasonId,
            LeagueSeasonDates seasonDates,
            bool hasGroups) : base(id, name)
        {
            Order = order;
            CategoryId = categoryId;
            CountryName = countryName;
            CountryCode = countryCode;
            IsInternational = isInternational;
            Region = region;
            SeasonId = seasonId;
            SeasonDates = seasonDates;
            HasGroups = hasGroups;
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

        public LeagueSeasonDates SeasonDates { get; private set; }

        public bool HasGroups { get; private set; }

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