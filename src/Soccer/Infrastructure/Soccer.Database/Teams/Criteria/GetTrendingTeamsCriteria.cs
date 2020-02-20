using Fanex.Data.Repository;
using Score247.Shared.Enumerations;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Database.Teams.Criteria
{
    public class GetTrendingTeamsCriteria : CriteriaBase
    {
        public GetTrendingTeamsCriteria(Language language)
        {
            Language = language.DisplayName;
            SportId = Sport.Soccer.Value;
        }

        public int SportId {get;}

        public string Language { get; }

        public override string GetSettingKey() => "Team_GetTrendingTeams";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Language);
    }
}
