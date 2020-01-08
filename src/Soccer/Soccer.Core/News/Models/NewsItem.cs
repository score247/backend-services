using System;
using MessagePack;
using Newtonsoft.Json;

namespace Soccer.Core.News.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class NewsItem
    {
        [SerializationConstructor, JsonConstructor]
        public NewsItem(string title, string content, string imageSource, string link, DateTime publishedDate)
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

        public DateTime PublishedDate { get; }
    }
}
