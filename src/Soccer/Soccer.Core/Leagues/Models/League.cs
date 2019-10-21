﻿using MessagePack;
using Score247.Shared.Base;

namespace Soccer.Core.Leagues.Models
{
    [MessagePackObject]
    public class League : BaseModel
    {
        public League()
        {
        }

        public League(
            string id,
            string name)
        {
            Id = id;
            Name = name;
        }

        [SerializationConstructor]
        public League(
            string id,
            string name,
            int order,
            string categoryId,
            string countryName,
            string countryCode,
            bool isInternational,
            string region) : this(id, name)
        {
            Order = order;
            CategoryId = categoryId;
            CountryName = countryName;
            CountryCode = countryCode;
            IsInternational = isInternational;
            Region = region;
        }

        [Key(2)]
        public int Order { get; private set; }

        [Key(3)]
        public string CategoryId { get; private set; }

        [Key(4)]
        public string CountryName { get; private set; }

        [Key(5)]
        public string CountryCode { get; private set; }

        [Key(6)]
        public bool IsInternational { get; private set; }

        [IgnoreMember]
        public int IsActive { get; private set; }

        [IgnoreMember]
        public string Region { get; private set; }

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