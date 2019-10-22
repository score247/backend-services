namespace Soccer.API.Matches
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Models;
    using Requests;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Teams.Models;

    public class MatchHandler :
        IRequestHandler<MatchesByDateRequest, IEnumerable<MatchSummary>>,
        IRequestHandler<MatchInfoByIdRequest, MatchInfo>,
        IRequestHandler<LiveMatchesRequest, IEnumerable<MatchSummary>>,
        IRequestHandler<LiveMatchCountRequest, int>,
        IRequestHandler<MatchCoverageByIdRequest, MatchCoverage>,
        IRequestHandler<MatchCommentaryByIdRequest, IEnumerable<MatchCommentary>>,
        IRequestHandler<MatchStatisticRequest, MatchStatistic>,
        IRequestHandler<MatchLineupsRequest, MatchLineups>
    {
        private readonly IMatchQueryService matchQueryService;

        public MatchHandler(IMatchQueryService matchQueryService)
        {
            this.matchQueryService = matchQueryService;
        }

        public async Task<IEnumerable<MatchSummary>> Handle(MatchesByDateRequest request, CancellationToken cancellationToken)
            => await matchQueryService.GetByDateRange(request.From, request.To, request.Language);

        public async Task<IEnumerable<MatchSummary>> Handle(LiveMatchesRequest request, CancellationToken cancellationToken)
            => await matchQueryService.GetLive(request.Language);

        public async Task<MatchInfo> Handle(MatchInfoByIdRequest request, CancellationToken cancellationToken)
            => await matchQueryService.GetMatchInfo(request.Id, request.Language);

        public async Task<int> Handle(LiveMatchCountRequest request, CancellationToken cancellationToken)
            => await matchQueryService.GetLiveMatchCount(request.Language);

        public async Task<MatchCoverage> Handle(MatchCoverageByIdRequest request, CancellationToken cancellationToken)
            => await matchQueryService.GetMatchCoverage(request.Id, request.Language);

        public Task<IEnumerable<MatchCommentary>> Handle(MatchCommentaryByIdRequest request, CancellationToken cancellationToken)
            => matchQueryService.GetMatchCommentary(request.Id, request.Language);

        public async Task<MatchStatistic> Handle(MatchStatisticRequest request, CancellationToken cancellationToken)
            => await matchQueryService.GetMatchStatistic(request.Id);

        public async Task<MatchLineups> Handle(MatchLineupsRequest request, CancellationToken cancellationToken)
        {
            var match = await matchQueryService.GetMatchLineups(request.Id, request.Language);
            // Please Ignore this temporaty code here
            return new MatchLineups(StubLineUp(true), StubLineUp(false), string.Empty);
        }

        private static Team StubLineUp(bool isHome)
            => new Team
            {
                IsHome = isHome,
                Formation = "4-4-1-1",
                Players = new List<Player>
                {
                    new Player { Name = "Foster, Ben", JerseyNumber = 1, Type = PlayerType.Goalkeeper, Position = Position.Goalkeeper, Order = 1 },
                    new Player { Name = "Nyom, Allan", JerseyNumber = 2, Type = PlayerType.Defender, Position = Position.RightBack, Order = 2  },
                    new Player { Name = "McAuley, Gareth", JerseyNumber = 23, Type = PlayerType.Defender, Position = Position.CentralDefender, Order = 3  },
                    new Player { Name = "Dawson, Craig", JerseyNumber = 25, Type = PlayerType.Defender, Position = Position.CentralDefender, Order = 4  },
                    new Player { Name = "Evans, Jonny", JerseyNumber = 6, Type = PlayerType.Defender, Position = Position.LeftBack, Order = 5  },
                    new Player { Name = "Phillips, Matt", JerseyNumber = 10, Type = PlayerType.Midfielder, Position = Position.RightWinger, Order = 6  },
                    new Player { Name = "Yacob, Claudio", JerseyNumber = 5, Type = PlayerType.Midfielder, Position = Position.CentralMidfielder, Order = 7  },
                    new Player { Name = "Fletcher, Darren", JerseyNumber = 24, Type = PlayerType.Midfielder, Position = Position.CentralMidfielder, Order = 8  },
                    new Player { Name = "McClean, James", JerseyNumber = 14, Type = PlayerType.Midfielder, Position = Position.LeftWinger, Order = 9  },
                    new Player { Name = "Chadli, Nacer", JerseyNumber = 22, Type = PlayerType.Midfielder, Position = Position.Striker, Order = 10  },
                    new Player { Name = "Rondon, Salomon", JerseyNumber = 9, Type = PlayerType.Forward, Position = Position.Striker, Order = 11  }
                },
                Substitutions = new List<Player>
                {
                    new Player { Name = "Myhill, Boaz", JerseyNumber = 13, Type = PlayerType.Goalkeeper },
                    new Player { Name = "Olsson, Jonas", JerseyNumber = 3, Type = PlayerType.Defender },
                    new Player { Name = "Gardner, Craig", JerseyNumber = 8, Type = PlayerType.Midfielder },
                    new Player { Name = "Morrison, James", JerseyNumber = 7, Type = PlayerType.Midfielder },
                    new Player { Name = "Berahino, Saido", JerseyNumber = 18, Type = PlayerType.Forward },
                    new Player { Name = "Leko, Jonathan", JerseyNumber = 45, Type = PlayerType.Forward },
                    new Player { Name = "Robson-Kanu, Hal", JerseyNumber = 4, Type = PlayerType.Forward }
                }
            };
    }
}