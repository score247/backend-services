using Fanex.Data.Repository;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Database.Teams
{
    public class GetHeadToHeadsCriteria : CriteriaBase
    {
        public GetHeadToHeadsCriteria(string homeTeamId, string awayTeamId, Language language)
        {
            HomeTeamId = homeTeamId;
            AwayTeamId = awayTeamId;
            Language = language.DisplayName;
        }

        public string HomeTeamId { get; }

        public string AwayTeamId { get; }

        public string Language { get; }

        public override string GetSettingKey() => "Team_GetHeadToHeads";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(HomeTeamId)
               && !string.IsNullOrWhiteSpace(Language)
               && !string.IsNullOrWhiteSpace(Language);
    }
}