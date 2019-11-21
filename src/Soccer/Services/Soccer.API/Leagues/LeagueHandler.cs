﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Soccer.API.Leagues.Requests;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Models;

namespace Soccer.API.Leagues
{
    public class LeagueHandler 
        : IRequestHandler<MajorLeaguesRequest, IEnumerable<League>>
        , IRequestHandler<UnprocessedLeagueSeasonRequest, IEnumerable<LeagueSeasonProcessedInfo>>
        , IRequestHandler<MatchesByLeagueRequest, IEnumerable<MatchSummary>>
    {
        private readonly ILeagueQueryService leagueQueryService;

        public LeagueHandler(ILeagueQueryService leagueQueryService)
        {
            this.leagueQueryService = leagueQueryService;
        }

        public Task<IEnumerable<League>> Handle(MajorLeaguesRequest request, CancellationToken cancellationToken)
            => leagueQueryService.GetMajorLeagues(request.Language);

        public Task<IEnumerable<LeagueSeasonProcessedInfo>> Handle(UnprocessedLeagueSeasonRequest request, CancellationToken cancellationToken)
        => leagueQueryService.GetLeagueSeasonFecth();

        public Task<IEnumerable<MatchSummary>> Handle(MatchesByLeagueRequest request, CancellationToken cancellationToken)
        => leagueQueryService.GetMatches(request.LeagueId, request.Language);
    }
}