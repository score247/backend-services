using System;

namespace Soccer.Core.News.Models
{
    public class NewsItem
    {
        public NewsItem(string title, string content, string imageSource, DateTimeOffset publishedDate)
        {
            Title = title;
            Content = content;
            ImageSource = imageSource;
            PublishedDate = publishedDate;
        }

        public string Title { get; }

        public string Content { get; }

        public string ImageSource { get; }

        public DateTimeOffset PublishedDate { get; }
    }
}
