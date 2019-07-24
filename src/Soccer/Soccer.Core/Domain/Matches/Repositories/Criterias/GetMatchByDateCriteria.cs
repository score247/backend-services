namespace Soccer.Core.Domain.Matches.Repositories.Criterias
{
    using System;
    using Fanex.Data.Repository;
    using Score247.Shared.Enumerations;

    public class GetMatchesByDateRangeCriteria : CriteriaBase
    {
        public GetMatchesByDateRangeCriteria(DateTime from, DateTime to, string language)
        {
            SportId = int.Parse(Sport.Soccer.Value);
            FromDate = from;
            ToDate = to;
            Language = language;
        }

        public int SportId { get; }

        public DateTime FromDate { get; }

        public DateTime ToDate { get; }

        public string Language { get; }

        public override string GetSettingKey() => "Score247_GetMatchesByDateRange";

        public override bool IsValid() => true;
    }
}