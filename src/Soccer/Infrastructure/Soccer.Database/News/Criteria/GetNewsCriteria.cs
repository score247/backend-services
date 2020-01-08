using Fanex.Data.Repository;
using Score247.Shared.Enumerations;

namespace Soccer.Database.News.Criteria
{
    public class GetNewsCriteria : CriteriaBase
    {
        public GetNewsCriteria(string language)
        {
            SportId = Sport.Soccer.Value;
            Language = language;
        }

        public byte SportId { get; }

        public string Language { get; }

        public override string GetSettingKey() => "News_GetNews";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Language);
    }
}
