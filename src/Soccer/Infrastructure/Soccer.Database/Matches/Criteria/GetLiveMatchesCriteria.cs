namespace Soccer.Database.Matches.Criteria
{
    using System;
    using Fanex.Data.Repository;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Shared.Enumerations;

    public class GetLiveMatchesCriteria : CriteriaBase
    {
        public GetLiveMatchesCriteria(
            Language language,
            DateTime fromDate)
        {
            SportId = Sport.Soccer.Value;
            Language = language.DisplayName;
            FromDate = fromDate.ToUniversalTime().Date;
        }

        public byte SportId { get; }

        public string Language { get; }

        public DateTime FromDate { get; } 

        public override string GetSettingKey() => "LiveMatch_GetBySportId";

        public override bool IsValid() => SportId > 0;
    }
}