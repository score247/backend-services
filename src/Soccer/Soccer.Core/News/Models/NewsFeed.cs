using System;

namespace Soccer.Core.News.Models
{
    public class NewsFeed
    {
        public NewsFeed(string title, string imageSource, string author, string source, DateTimeOffset publishedAt)
        {
            Title = title;
            ImageSource = imageSource;
            Author = author;
            Source = source;
            PublishedAt = publishedAt;
        }     

        public string Title { get; }

        public string Content { get; }

        public string ImageSource { get; }

        public string Author { get; }

        public string Source { get; }

        public DateTimeOffset PublishedAt { get; }
    }
}
