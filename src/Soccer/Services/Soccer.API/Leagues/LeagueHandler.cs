﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Soccer.API.Leagues.Requests;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Models;

namespace Soccer.API.Leagues
{
    public class LeagueHandler :
        IRequestHandler<MajorLeaguesRequest, IEnumerable<League>>,
        IRequestHandler<UnprocessedLeagueSeasonRequest, IEnumerable<LeagueSeasonProcessedInfo>>,
        IRequestHandler<MatchesByLeagueRequest, IEnumerable<MatchSummary>>,
        IRequestHandler<LeagueTableRequest, LeagueTable>,
        IRequestHandler<CountryLeaguesRequest, IEnumerable<League>>
    {
        private readonly ILeagueQueryService leagueQueryService;

        public LeagueHandler(ILeagueQueryService leagueQueryService)
        {
            this.leagueQueryService = leagueQueryService;
        }

        public Task<IEnumerable<League>> Handle(MajorLeaguesRequest request, CancellationToken cancellationToken)
            => leagueQueryService.GetMajorLeagues(request.Language);

        public Task<IEnumerable<LeagueSeasonProcessedInfo>> Handle(UnprocessedLeagueSeasonRequest request, CancellationToken cancellationToken)
            => leagueQueryService.GetLeagueSeasonFetch();

        public Task<IEnumerable<MatchSummary>> Handle(MatchesByLeagueRequest request, CancellationToken cancellationToken)
            => leagueQueryService.GetMatches(request.LeagueId, request.LeagueGroupName, request.Language);

        public Task<LeagueTable> Handle(LeagueTableRequest request, CancellationToken cancellationToken)
            => leagueQueryService.GetLeagueTable(request.LeagueId, request.SeasonId, request.GroupName, request.Language);

        public Task<IEnumerable<League>> Handle(CountryLeaguesRequest request, CancellationToken cancellationToken)
            => leagueQueryService.GetCountryLeagues(request.CountryCode, request.Language);
    }
}