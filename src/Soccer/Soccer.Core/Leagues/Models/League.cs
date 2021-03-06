using System;
using System.Collections.Generic;
using MessagePack;
using Newtonsoft.Json;
using Score247.Shared.Base;
using Soccer.Core.Teams.Models;

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
            long hasGroups = 0,
            string abbreviation = "")
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
                    Convert.ToBoolean(hasGroups),
                    abbreviation)
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
                  league.HasGroups,
                  league.Abbreviation)
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
            bool hasGroups,
            string abbreviation,
            IList<TeamProfile> teams = null) : base(id, name)
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
            Abbreviation = abbreviation;
            Teams = teams;
        }

#pragma warning restore S107 // Methods should not have too many parameters

#pragma warning disable S109 // Magic numbers should not be used

        public int Order { get; private set; }

        public string CategoryId { get; }

        public string CountryName { get; private set; }

        public string CountryCode { get; private set; }

        public bool IsInternational { get; private set; }

        public string Region { get; private set; }

        public string SeasonId { get; private set; }

        public LeagueSeasonDates SeasonDates { get; private set; }

        public bool HasGroups { get; private set; }

        public string Abbreviation { get; private set; }

        public IList<TeamProfile> Teams { get; private set; }

        public void SetAbbreviation(string abbreviation)
        {
            Abbreviation = abbreviation;
        }

#pragma warning restore S109 // Magic numbers should not be used

        public void UpdateLeague(string countryCode, bool isInternational, int order, string region, bool hasGroups)
        {
            CountryCode = countryCode;
            IsInternational = isInternational;
            Order = order;
            Region = region;
            HasGroups = hasGroups;
        }

        public void UpdateHasGroups(bool hasGroups)
        {
            HasGroups = hasGroups;
        }

        public void UpdateCountryAndLeagueName(string name, string countryName)
        {
            Name = name;
            CountryName = countryName;
        }

        public void UpdateTeams(IList<TeamProfile> teams)
        {
            Teams = teams;
        }
    }
}