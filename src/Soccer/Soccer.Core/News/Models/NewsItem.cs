using System;
using System.IO;
using System.Linq;
using MessagePack;
using Newtonsoft.Json;

namespace Soccer.Core.News.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class NewsItem
    {
        [SerializationConstructor, JsonConstructor]
        public NewsItem(string title, string content, string imageSource, string link, string author, DateTime publishedDate, string provider)
        {
            Title = title;
            Content = content;
            ImageSource = imageSource;
            Link = link;
            Author = author;
            PublishedDate = publishedDate;
            Provider = provider;
        }

        public string Title { get; }

        public string Content { get; }

        public string ImageSource { get; }

        public string Link { get; }

        public string Author { get; }

        public DateTime PublishedDate { get; }

        public string ImageName => string.IsNullOrWhiteSpace(ImageSource) ? string.Empty : GenerateImageName();

        public string Provider { get; }

        private string GenerateImageName()
        => $"{Link.Split(Path.AltDirectorySeparatorChar).LastOrDefault()}-{ImageSource.Split(Path.AltDirectorySeparatorChar).LastOrDefault()}";
    }
}
