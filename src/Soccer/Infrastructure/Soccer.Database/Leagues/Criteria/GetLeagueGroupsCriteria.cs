using Fanex.Data.Repository;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Database.Leagues.Criteria
{
    public class GetLeagueGroupsCriteria : CriteriaBase
    {
        public GetLeagueGroupsCriteria(string leagueId, Language language)
        {
            LeagueId = leagueId;
            LanguageCode = language?.DisplayName;
        }

        public string LeagueId { get; }

        public string LanguageCode { get; }

        public override string GetSettingKey()
            => "League_GetLeagueGroups";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(LeagueId)
            && !string.IsNullOrWhiteSpace(LanguageCode);
    }
}