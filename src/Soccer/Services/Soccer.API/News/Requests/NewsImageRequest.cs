using MediatR;

namespace Soccer.API.News.Requests
{
    public class NewsImageRequest : IRequest<byte[]>
    {
        public NewsImageRequest(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
