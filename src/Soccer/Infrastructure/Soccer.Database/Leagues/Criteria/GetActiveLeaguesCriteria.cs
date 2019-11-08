using Fanex.Data.Repository;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Database.Leagues.Criteria
{
    public class GetActiveLeaguesCriteria : CriteriaBase
    {
        public GetActiveLeaguesCriteria(string language = Language.English)
        {
            LanguageCode = language;
        }

        public string LanguageCode { get; }

        public override string GetSettingKey() => "League_GetActive";

        public override bool IsValid() => true;
    }
}