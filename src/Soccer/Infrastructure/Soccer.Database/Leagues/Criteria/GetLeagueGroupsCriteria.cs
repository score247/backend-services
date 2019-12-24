using Fanex.Data.Repository;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Database.Leagues.Criteria
{
    public class GetLeagueGroupsCriteria : CriteriaBase
    {
        public GetLeagueGroupsCriteria(string leagueId, string seasonId, Language language)
        {
            LeagueId = leagueId;
            SeasonId = seasonId;
            LanguageCode = language?.DisplayName;
        }

        public string LeagueId { get; }

        public string SeasonId { get; }

        public string LanguageCode { get; }

        public override string GetSettingKey()
            => "League_GetGroupStages";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(LeagueId)
            && !string.IsNullOrWhiteSpace(LanguageCode);
    }
}