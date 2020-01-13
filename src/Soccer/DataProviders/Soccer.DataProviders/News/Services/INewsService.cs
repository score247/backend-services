using System.Collections.Generic;
using System.Threading.Tasks;
using Soccer.Core.News.Models;

namespace Soccer.DataProviders.News.Services
{
    public interface INewsService
    {
        Task<List<NewsItem>> GetNewsFeed(string feedUri);

        Task<NewsImage> GetNewsImage(string imageName, string imageLink);
    }
}
