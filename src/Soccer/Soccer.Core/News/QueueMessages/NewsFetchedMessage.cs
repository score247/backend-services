using System.Collections.Generic;
using Soccer.Core.News.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core.News.QueueMessages
{
    public interface INewsFetchedMessage
    {
        IEnumerable<NewsItem> NewsList { get; }

        Language Language { get; }
    }

    public class NewsFetchedMessage : INewsFetchedMessage
    {
        public NewsFetchedMessage(IEnumerable<NewsItem> newsList, Language language)
        {
            NewsList = newsList;
            Language = language;
        }

        public IEnumerable<NewsItem> NewsList { get; private set; }

        public Language Language { get; private set; }
    }
}
