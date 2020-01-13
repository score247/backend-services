using Score247.Shared.Enumerations;

namespace Soccer.Database.News.Commands
{
    public class InsertNewsImageCommand : BaseCommand
    {
        public InsertNewsImageCommand(string name, string content)
         {
            SportId = Sport.Soccer.Value;
            ImageName = name;
            ImageContent = content;
        }

        public byte SportId { get; }

        public string ImageName { get; }

        public string ImageContent { get; }

        public override string GetSettingKey() => "News_InsertOrUpdateImage";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(ImageName) && ImageContent != null;
    }
}
