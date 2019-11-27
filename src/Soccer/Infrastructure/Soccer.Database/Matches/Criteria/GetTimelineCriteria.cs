using System;
using Soccer.Database._Shared.Extensions;

namespace Soccer.Database.Matches.Criteria
{
    public class GetTimelineEventsCriteria : CustomCriteria
    {
        public GetTimelineEventsCriteria(string matchId, DateTimeOffset eventDate = default) : base(eventDate)
        {
            MatchId = matchId;
        }

        public string MatchId { get; }

        public override string GetSettingKey() => "Match_GetTimelineEvents".GetCorrespondingKey(EventDate);

        public override bool IsValid() => !string.IsNullOrEmpty(MatchId);
    }
}