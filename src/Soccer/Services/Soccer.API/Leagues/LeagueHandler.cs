using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Soccer.API.Leagues.Requests;
using Soccer.Core.Leagues.Models;

namespace Soccer.API.Leagues
{
    public class LeagueHandler : IRequestHandler<MajorLeaguesRequest, IEnumerable<League>>
    {
        private readonly ILeagueQueryService leagueQueryService;

        public LeagueHandler(ILeagueQueryService leagueQueryService)
        {
            this.leagueQueryService = leagueQueryService;
        }

        public Task<IEnumerable<League>> Handle(MajorLeaguesRequest request, CancellationToken cancellationToken)
            => leagueQueryService.GetMajorLeagues(request.Language);
    }
}