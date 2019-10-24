using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Soccer.API.Teams.Requests;
using Soccer.Core.Matches.Models;

namespace Soccer.API.Teams
{
    public class TeamHandler : IRequestHandler<HeadToHeadRequest, IEnumerable<MatchSummary>>
    {
        private readonly ITeamQueryService teamQueryService;

        public TeamHandler(ITeamQueryService teamQueryService)
        {
            this.teamQueryService = teamQueryService;
        }

        public Task<IEnumerable<MatchSummary>> Handle(HeadToHeadRequest request, CancellationToken cancellationToken)
            => teamQueryService.GetHeadToHeads(request.HomeTeamId, request.AwayTeamId, request.Language);
    }
}