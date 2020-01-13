using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Soccer.API.News.Requests;
using Soccer.Core.News.Models;

namespace Soccer.API.News
{
    public class NewsHandler :
        IRequestHandler<NewsRequest, IEnumerable<NewsItem>>,
        IRequestHandler<NewsImageRequest, byte[]>
    {
        private readonly INewsQueryService newsQueryService;

        public NewsHandler(INewsQueryService newsQueryService) 
        {
            this.newsQueryService = newsQueryService;
        }

        public Task<IEnumerable<NewsItem>> Handle(NewsRequest request, CancellationToken cancellationToken)
        => newsQueryService.GetNews(request.Language);

        public async Task<byte[]> Handle(NewsImageRequest request, CancellationToken cancellationToken)
        {
            return await newsQueryService.GetNewsImage(request.Name);

        }
    }
}
