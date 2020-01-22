using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Soccer.API.Favorites.Requests;

namespace Soccer.API.Favorites
{
    public class FavoriteHandler :
        IRequestHandler<AddFavoriteRequest, bool>
    {
        private readonly IFavoriteCommandService favoriteCommandService;

        public FavoriteHandler(
            IFavoriteCommandService favoriteCommandService)
        {
            this.favoriteCommandService = favoriteCommandService;
        }

        public async Task<bool> Handle(AddFavoriteRequest request, CancellationToken cancellationToken)
        => (await favoriteCommandService.AddFavorite(request.UserFavorite)) > 0; 
    }
}
