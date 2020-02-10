using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using Soccer.Database.Favorites.Criteria;

namespace Soccer.API.Favorites
{
    public interface IFavoriteQueryService
    {
        Task<IEnumerable<string>> GetUsersByFavoriteId(string matchId);
    }

    public class FavoriteQueryService : IFavoriteQueryService
    {
        private readonly IDynamicRepository dynamicRepository;

        public FavoriteQueryService(IDynamicRepository dynamicRepository)
        => this.dynamicRepository = dynamicRepository;

        Task<IEnumerable<string>> IFavoriteQueryService.GetUsersByFavoriteId(string matchId)
        => dynamicRepository.FetchAsync<string>(new GetUsersByFavoriteIdCriteria(matchId));
    }
}
