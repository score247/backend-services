using System;
using System.Collections.Generic;
using System.Text;
using Fanex.Data.Repository;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Database.Timelines.Criteria
{
    public class GetCommentaryCriteria : CriteriaBase
    {
        public GetCommentaryCriteria(string matchId, Language language) 
        {
            MatchId = matchId;
            Language = language.DisplayName;
        }

        public string MatchId { get; }

        public string Language { get; }

        public override string GetSettingKey() => "Match_GetCommentaries";

        public override bool IsValid() => !string.IsNullOrEmpty(MatchId);
    }
}
