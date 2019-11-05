using Fanex.Data.Repository;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Database.Teams
{
    public class GetTeamResultsCriteria : CriteriaBase
    {
        public GetTeamResultsCriteria(string teamId, Language language)
        {
            TeamId = teamId;
            Language = language.DisplayName;
        }

        public string TeamId { get; }

        public string Language { get; }

        public override string GetSettingKey() => "Team_GetTeamResults";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(TeamId)
               && !string.IsNullOrWhiteSpace(Language);
    }
}