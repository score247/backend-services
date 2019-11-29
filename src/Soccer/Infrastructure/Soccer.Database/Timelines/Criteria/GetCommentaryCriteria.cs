using System;
using Soccer.Core.Shared.Enumerations;
using Soccer.Database._Shared.Extensions;

namespace Soccer.Database.Timelines.Criteria
{
    public class GetCommentaryCriteria : CustomCriteria
    {
        public GetCommentaryCriteria(string matchId, Language language, DateTimeOffset eventDate = default) : base(eventDate)
        {
            MatchId = matchId;
            Language = language.DisplayName;
        }

        public string MatchId { get; }

        public string Language { get; }

        public override string GetSettingKey() => "Match_GetCommentaries".GetCorrespondingKey(EventDate, DateTimeOffset.Now);

        public override bool IsValid() => !string.IsNullOrEmpty(MatchId);
    }
}