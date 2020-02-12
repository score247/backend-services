using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Soccer.API.Favorites.Requests;

namespace Soccer.API.Favorites
{
    public class FavoriteHandler :
        IRequestHandler<AddFavoriteRequest, bool>,
        IRequestHandler<RemoveFavoriteRequest, bool>,
        IRequestHandler<GetUsersByFavoriteRequest, IReadOnlyList<string>>,
        IRequestHandler<SyncFavoriteRequest, bool>
    { 
        private readonly IFavoriteCommandService favoriteCommandService;
        private readonly IFavoriteQueryService favoriteQueryService;

        public FavoriteHandler(
            IFavoriteCommandService favoriteCommandService,
            IFavoriteQueryService favoriteQueryService)
        {
            this.favoriteCommandService = favoriteCommandService;
            this.favoriteQueryService = favoriteQueryService;
        }

        public async Task<bool> Handle(AddFavoriteRequest request, CancellationToken cancellationToken)
        => (await favoriteCommandService.AddFavorite(request.UserFavorite)) > 0;

        public async Task<bool> Handle(RemoveFavoriteRequest request, CancellationToken cancellationToken)
        => (await favoriteCommandService.RemoveFavorite(request.UserFavorite)) > 0;

        public async Task<IReadOnlyList<string>> Handle(GetUsersByFavoriteRequest request, CancellationToken cancellationToken)
        => (await favoriteQueryService.GetUsersByFavoriteId(request.MatchId))?.ToList();

        public async Task<bool> Handle(SyncFavoriteRequest request, CancellationToken cancellationToken)
        {
            await favoriteCommandService.RemoveFavorite(request.SyncUserFavorite?.RemovedUserFavorite);

            return (await favoriteCommandService.AddFavorite(request.SyncUserFavorite?.AddedUserFavorite)) > 0;
        }
    }
}
