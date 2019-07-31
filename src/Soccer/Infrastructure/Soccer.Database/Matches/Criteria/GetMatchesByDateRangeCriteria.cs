namespace Soccer.Database.Matches.Criteria
{
    using System;
    using Fanex.Data.Repository;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Shared.Enumerations;

    public class GetMatchesByDateRangeCriteria : CriteriaBase
    {
        public GetMatchesByDateRangeCriteria(DateTime from, DateTime to, Language language)
        {
            SportId = Sport.Soccer.Value;
            FromDate = from.ToUniversalTime();
            ToDate = to.ToUniversalTime();
            Language = language.DisplayName;
        }

        public int SportId { get; }

        public DateTime FromDate { get; }

        public DateTime ToDate { get; }

        public string Language { get; }

        public override string GetSettingKey() => "Score247_GetMatchesByDateRange";

        public override bool IsValid() => SportId > 0 && FromDate <= ToDate;
    }
}