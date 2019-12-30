using System.Collections.Generic;
using System.Threading.Tasks;
using Soccer.Core.News.Models;

namespace Soccer.DataProviders.News.Services
{
    public interface INewsService
    {
        Task<List<NewsFeed>> GetNewsFeed();
    }
}
