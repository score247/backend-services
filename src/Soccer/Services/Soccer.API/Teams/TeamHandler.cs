using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Soccer.API.Teams.Requests;
using Soccer.Core.Matches.Models;
using Soccer.Core.Teams.Models;

namespace Soccer.API.Teams
{
    public class TeamHandler :
        IRequestHandler<HeadToHeadRequest, IEnumerable<MatchSummary>>,
        IRequestHandler<TeamResultRequest, IEnumerable<MatchSummary>>,
        IRequestHandler<SearchTeamByNameRequest, IEnumerable<TeamProfile>>,
        IRequestHandler<TrendingTeamsRequest, IEnumerable<TeamProfile>>
    {
        private readonly ITeamQueryService teamQueryService;

        public TeamHandler(ITeamQueryService teamQueryService)
        {
            this.teamQueryService = teamQueryService;
        }

        public Task<IEnumerable<MatchSummary>> Handle(HeadToHeadRequest request, CancellationToken cancellationToken)
            => teamQueryService.GetHeadToHeads(request.HomeTeamId, request.AwayTeamId, request.Language);

        public Task<IEnumerable<MatchSummary>> Handle(TeamResultRequest request, CancellationToken cancellationToken)
            => teamQueryService.GetTeamResults(request.TeamId, request.OpponentTeamId, request.Language);

        public Task<IEnumerable<TeamProfile>> Handle(SearchTeamByNameRequest request, CancellationToken cancellationToken)
            => teamQueryService.SearchTeamByName(request.Keyword, request.Language);

        public Task<IEnumerable<TeamProfile>> Handle(TrendingTeamsRequest request, CancellationToken cancellationToken)
            => teamQueryService.GetTrendingTeams(request.Language);
    }
}