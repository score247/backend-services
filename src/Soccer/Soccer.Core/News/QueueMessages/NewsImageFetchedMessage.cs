namespace Soccer.Core.News.QueueMessages
{
    public interface INewsImageFetchedMessage
    {
        string ImageName { get; }

        string ImageContent { get; }
    }

    public class NewsImageFetchedMessage : INewsImageFetchedMessage
    {
        public NewsImageFetchedMessage(string imageName, string imageContent) 
        {
            ImageName = imageName;
            ImageContent = imageContent;
        }

        public string ImageName { get; }

        public string ImageContent { get; }
    }
}
