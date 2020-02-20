using System;
using Fanex.Data.Repository;
using Score247.Shared.Enumerations;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Database.Teams.Criteria
{
    public class GetMatchesByTeamCriteria : CriteriaBase
    {
        public GetMatchesByTeamCriteria(string teamId, Language language)
        {
            TeamId = teamId;
            Language = language.DisplayName;
            SportId = Sport.Soccer.Value;
        }

        public byte SportId { get; }

        public string TeamId { get; }

        public string Language { get; }

        public override string GetSettingKey() => "Match_GetByTeamId";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(TeamId);
    }
}
