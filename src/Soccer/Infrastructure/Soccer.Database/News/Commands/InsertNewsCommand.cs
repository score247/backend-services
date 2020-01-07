using System.Collections.Generic;
using Score247.Shared.Enumerations;
using Soccer.Core.News.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Database.News.Commands
{
    public class InsertNewsCommand : BaseCommand
    {
        public InsertNewsCommand(
            IEnumerable<NewsItem> newsList,
            Language language) 
        {
            SportId = Sport.Soccer.Value;
            News = ToJsonString(newsList);
            Language = language.DisplayName;
        }

        public byte SportId { get; }

        public string News { get; }

        public string Language { get; }

        public override string GetSettingKey() => "News_InsertOrUpdateNews";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(News) && !string.IsNullOrWhiteSpace(Language);
    }
}
