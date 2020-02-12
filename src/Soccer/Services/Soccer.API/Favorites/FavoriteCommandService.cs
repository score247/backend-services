using System.Threading.Tasks;
using Fanex.Data.Repository;
using Soccer.Core.Models.Favorites;
using Soccer.Database.Favorites.Commands;

namespace Soccer.API.Favorites
{
    public interface IFavoriteCommandService
    {
        Task<int> AddFavorite(UserFavorite userFavorite);

        Task<int> RemoveFavorite(UserFavorite userFavorite);
    }

    public class FavoriteCommandService : IFavoriteCommandService
    {
        private readonly IDynamicRepository dynamicRepository;

        public FavoriteCommandService(IDynamicRepository dynamicRepository)
        => this.dynamicRepository = dynamicRepository;

        public Task<int> AddFavorite(UserFavorite userFavorite)
        => dynamicRepository.ExecuteAsync(new InsertOrUpdateFavoriteCommand(userFavorite));

        public Task<int> RemoveFavorite(UserFavorite userFavorite)
         => dynamicRepository.ExecuteAsync(new RemoveFavoriteCommand(userFavorite));
    }
}