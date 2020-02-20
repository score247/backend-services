using System;
using Fanex.Data.Repository;
using Score247.Shared.Enumerations;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Database.Teams.Criteria
{
    public class SearchTeamByNameCriteria : CriteriaBase
    {
        public SearchTeamByNameCriteria(string keyword, Language language)
        {
            Keyword = keyword;
            Language = language.DisplayName;
            SportId = Sport.Soccer.Value;
        }

        public int SportId { get; }

        public string Language { get; }

        public string Keyword { get; }

        public override string GetSettingKey() => "Team_SearchByName";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Keyword) && !string.IsNullOrWhiteSpace(Language);
    }
}
