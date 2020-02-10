using System.Collections.Generic;
using MediatR;

namespace Soccer.API.Favorites.Requests
{
    public class GetUsersByFavoriteRequest : IRequest<IReadOnlyList<string>>
    {
        public GetUsersByFavoriteRequest(string matchId) 
        {
            MatchId = matchId;
        }

        public string MatchId { get; }
    }
}
