namespace Soccer.Database.Matches.Criteria
{
    using Fanex.Data.Repository;
    using Soccer.Core.Shared.Enumerations;

    public class GetMatchByIdCriteria : CriteriaBase
    {
        public GetMatchByIdCriteria(string id, Language language)
        {
            Id = id;
            Language = language.DisplayName;
        }

        public string Id { get; }

        public string Language { get; }

        public override string GetSettingKey() => "Score247_GetMatchById";

        public override bool IsValid() => string.IsNullOrEmpty(Id);
    }
}