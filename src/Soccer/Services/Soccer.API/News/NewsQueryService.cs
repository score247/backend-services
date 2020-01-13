using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using Soccer.Core.News.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Database.News.Criteria;

namespace Soccer.API.News
{
    public interface INewsQueryService
    {
        Task<IEnumerable<NewsItem>> GetNews(Language language);

        Task<byte[]> GetNewsImage(string imageName);
    }

    public class NewsQueryService : INewsQueryService
    {
        private readonly IDynamicRepository dynamicRepository;

        public NewsQueryService(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public Task<IEnumerable<NewsItem>> GetNews(Language language)
        => dynamicRepository.FetchAsync<NewsItem>(new GetNewsCriteria(language.DisplayName));

        public Task<byte[]> GetNewsImage(string imageName)
        => dynamicRepository.GetAsync<byte[]>(new GetNewsImageCriteria(imageName));
    }
}
