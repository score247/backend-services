namespace Soccer.Database.Matches.Criteria
{
    using Fanex.Data.Repository;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Shared.Enumerations;

    public class GetLiveMatchesCriteria : CriteriaBase
    {
        public GetLiveMatchesCriteria(
            Language language)
        {
            SportId = Sport.Soccer.Value;
            Language = language.DisplayName;
        }

        public byte SportId { get; }

        public string Language { get; }

        public override string GetSettingKey() => "LiveMatch_GetBySportId";

        public override bool IsValid() => SportId > 0;
    }
}