namespace Soccer.Core.News.Models
{
    public class NewsImage
    {
        public NewsImage(string name, string content)
        {
            Name = name;
            Content = content;
        }

        public string Name { get; }

        public string Content { get; }
    }
}
