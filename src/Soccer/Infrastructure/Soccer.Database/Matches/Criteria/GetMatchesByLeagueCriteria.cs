using Fanex.Data.Repository;
using Score247.Shared.Enumerations;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Database.Matches.Criteria
{
    public class GetMatchesByLeagueCriteria : CriteriaBase
    {
        public GetMatchesByLeagueCriteria(string leagueId, Language language)
        {
            SportId = Sport.Soccer.Value;
            LeagueId = leagueId;
            Language = language.DisplayName;
        }

        public int SportId { get; }

        public string  LeagueId { get; }

        public string Language { get; }

        public override string GetSettingKey() => "Match_GetByLeagueId";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(LeagueId);
    }
}
