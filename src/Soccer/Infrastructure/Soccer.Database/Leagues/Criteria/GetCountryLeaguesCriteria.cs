using System;
using Fanex.Data.Repository;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Database.Leagues.Criteria
{
    public class GetCountryLeaguesCriteria : CriteriaBase
    {
        private const string InternationalCode = "INTL";

        public GetCountryLeaguesCriteria(string countryName, Language language)
        {
            CountryCode = countryName;
            LanguageCode = language?.DisplayName;
        }

        public string CountryCode { get; }

        public bool IsInternational =>
            CountryCode.Equals(InternationalCode, StringComparison.InvariantCultureIgnoreCase);

        public string LanguageCode { get; }

        public override string GetSettingKey()
            => "League_GetCountryLeagues";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(LanguageCode);
    }
}