namespace Soccer.API.Matches
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Models;
    using Requests;
    using Soccer.API.Matches.Helpers;
    using Soccer.Core.Matches.Models;

    public class MatchHandler :
        IRequestHandler<MatchesByDateRequest, IEnumerable<MatchSummary>>,
        IRequestHandler<MatchInfoByIdRequest, MatchInfo>,
        IRequestHandler<LiveMatchesRequest, IEnumerable<MatchSummary>>,
        IRequestHandler<LiveMatchCountRequest, int>,
        IRequestHandler<MatchCoverageByIdRequest, MatchCoverage>,
        IRequestHandler<MatchCommentaryByIdRequest, IEnumerable<MatchCommentary>>,
        IRequestHandler<MatchStatisticRequest, MatchStatistic>,
        IRequestHandler<MatchLineupsRequest, MatchPitchViewLineups>
    {
        private readonly IMatchQueryService matchQueryService;
        private readonly IMatchLineupsGenerator matchLineupsGenerator;

        public MatchHandler(
            IMatchQueryService matchQueryService,
            IMatchLineupsGenerator matchLineupsGenerator)
        {
            this.matchQueryService = matchQueryService;
            this.matchLineupsGenerator = matchLineupsGenerator;
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

        public async Task<MatchPitchViewLineups> Handle(MatchLineupsRequest request, CancellationToken cancellationToken)
        {
            var matchLineups = await matchQueryService.GetMatchLineups(request.Id, request.Language);

            var lineupsSvg = matchLineupsGenerator.Generate(matchLineups);

            var folder = $"{AppDomain.CurrentDomain.BaseDirectory}/data/";
            System.IO.Directory.CreateDirectory(folder);
            File.WriteAllText(Path.Combine(folder, $"{request.Id.Replace(":", "_")}.svg"), lineupsSvg);

            return new MatchPitchViewLineups(
                matchLineups.Id,
                matchLineups.EventDate,
                matchLineups.Home,
                matchLineups.Away,
                lineupsSvg);
        }
    }
}