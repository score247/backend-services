using System;
using System.Collections.Generic;
using System.Text;
using Soccer.Core.Shared.Enumerations;
using Soccer.Database._Shared.Extensions;

namespace Soccer.Database.Matches.Criteria
{
    public class GetMatchTimelineEventsCriteria : CustomCriteria
    {
        public GetMatchTimelineEventsCriteria(string matchId, Language language, DateTimeOffset eventDate = default) : base(eventDate)
        {
            MatchId = matchId;
            Language = language.DisplayName;
        }

        public string MatchId { get; }

        public string Language { get; }

        public override string GetSettingKey() => "Match_GetMainTimelineEvents".GetCorrespondingKey(EventDate, DateTimeOffset.Now);

        public override bool IsValid() => !string.IsNullOrEmpty(MatchId) ;
    }
}
