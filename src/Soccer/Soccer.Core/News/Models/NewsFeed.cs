using System;

namespace Soccer.Core.News.Models
{
    public class NewsFeed
    {
        public NewsFeed(string guid, string title, string source, string summary, DateTimeOffset publishedDate)
        {
            Guid = guid;
            Title = title;
            Source = source;
            Summary = summary;
            PublishedDate = publishedDate;
        }

        public string Guid { get; }

        public string Title { get; }

        public string Source { get; }

        public string Summary { get; }

        public DateTimeOffset PublishedDate { get; }
    }
}
