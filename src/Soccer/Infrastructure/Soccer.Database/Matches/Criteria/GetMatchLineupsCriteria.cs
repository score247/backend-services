using System;
using Soccer.Core.Shared.Enumerations;
using Soccer.Database._Shared.Extensions;

namespace Soccer.Database.Matches.Criteria
{
    public class GetMatchLineupsCriteria : CustomCriteria
    {
        public GetMatchLineupsCriteria(string id, Language language, DateTimeOffset eventDate) : base(eventDate)
        {
            MatchId = id;
            Language = language.DisplayName;
        }

        public string MatchId { get; }

        public string Language { get; }

        public override string GetSettingKey()
            => "Match_GetLineups".GetCorrespondingKey(EventDate);

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(MatchId)
            && !string.IsNullOrWhiteSpace(Language);
    }
}