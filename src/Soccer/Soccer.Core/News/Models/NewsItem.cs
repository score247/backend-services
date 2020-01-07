using System;

namespace Soccer.Core.News.Models
{
    public class NewsItem
    {
        public NewsItem(string title, string content, string imageSource, string link, DateTimeOffset publishedDate)
        {
            Title = title;
            Content = content;
            ImageSource = imageSource;
            Link = link;
            PublishedDate = publishedDate;
        }

        public string Title { get; }

        public string Content { get; }

        public string ImageSource { get; }

        public string Link { get; }

        public DateTimeOffset PublishedDate { get; }
    }
}
