using Fanex.Data.Repository;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Database.Matches.Criteria
{
    public class GetMatchLineupsCriteria : CriteriaBase
    {
        public GetMatchLineupsCriteria(string id, Language language)
        {
            MatchId = id;
            Language = language.DisplayName;
        }

        public string MatchId { get; }

        public string Language { get; }

        public override string GetSettingKey()
            => "Match_GetLineups";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(MatchId)
            && !string.IsNullOrWhiteSpace(Language);
    }
}