namespace Soccer.Core.News.QueueMessages
{
    public interface INewsImageFetchedMessage
    {
        string ImageName { get; }

        byte[] ImageContent { get; }
    }

    public class NewsImageFetchedMessage : INewsImageFetchedMessage
    {
        public NewsImageFetchedMessage(string imageName, byte[] imageContent) 
        {
            ImageName = imageName;
            ImageContent = imageContent;
        }

        public string ImageName { get; }

        public byte[] ImageContent { get; }
    }
}
