namespace Soccer.Core.News.Models
{
    public class NewsImage
    {
        public NewsImage(string name, byte[] content)
        {
            Name = name;
            Content = content;
        }

        public string Name { get; }

        public byte[] Content { get; }
    }
}
