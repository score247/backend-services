using Fanex.Data.Repository;
using Score247.Shared.Enumerations;

namespace Soccer.Database.News.Criteria
{
    public class GetNewsImageCriteria : CriteriaBase
    {
        public GetNewsImageCriteria(string imageName)
        {
            SportId = Sport.Soccer.Value;
            ImageName = imageName;
        }

        public byte SportId { get; }

        public string ImageName { get; }

        public override string GetSettingKey() => "News_GetImageByName";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(ImageName);
    }
}
