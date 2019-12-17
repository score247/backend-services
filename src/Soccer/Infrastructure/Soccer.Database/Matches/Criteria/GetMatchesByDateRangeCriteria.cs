using Score247.Shared.Enumerations;
using Soccer.Core.Shared.Enumerations;
using Soccer.Database._Shared.Extensions;
using System;

namespace Soccer.Database.Matches.Criteria
{
    public class GetMatchesByDateRangeCriteria : CustomCriteria
    {
        public GetMatchesByDateRangeCriteria(
            DateTimeOffset from,
            DateTimeOffset to,
            Language language) : base(from)
        {
            SportId = Sport.Soccer.Value;
            FromDate = from.ToUniversalTime();
            ToDate = to.ToUniversalTime();
            Language = language.DisplayName;
        }

        public int SportId { get; }

        public DateTimeOffset FromDate { get; }

        public DateTimeOffset ToDate { get; }

        public string Language { get; }

        public override string GetSettingKey() => "Match_GetByDateRange".GetCorrespondingKey(EventDate, DateTimeOffset.Now);

        public override bool IsValid() => SportId > 0 && FromDate <= ToDate;
    }
}