namespace Soccer.API.Modules.Matches
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Ardalis.GuardClauses;
    using Score247.Shared.Enumerations;
    using Soccer.API.Configurations;
    using Soccer.Core.Domain.Matches;
    using Soccer.Core.Domain.Matches.Models;
    using Soccer.Core.Domain.Matches.Specifications;

    public interface IMatchQueryService
    {
        Task<IEnumerable<Match>> GetByDateRange(DateTime from, DateTime to, TimeSpan clientTimeOffset, string language);

        Task<IEnumerable<Match>> GetLive(TimeSpan clientTimeOffset, string language);
    }

    public class MatchQueryService : IMatchQueryService
    {
        private readonly IMatchRepository matchRepository;
        private readonly ILiveMatchRepository liveMatchRepository;
        private readonly IAppSettings appSettings;

        public MatchQueryService(
            IAppSettings appSettings,
            IMatchRepository matchRepository,
            ILiveMatchRepository liveMatchRepository)
        {
            this.matchRepository = matchRepository;
            this.liveMatchRepository = liveMatchRepository;
            this.appSettings = appSettings;
        }

        public async Task<IEnumerable<Match>> GetLive(TimeSpan clientTimeOffset, string language)
        {
            var liveMatchSpec = new GetLiveMatchSpecification(int.Parse(Sport.Soccer.Value), clientTimeOffset, language);
            var matches = await liveMatchRepository.ListAsync(liveMatchSpec);

            return matches.Select(m => m.Match);
        }

        public async Task<IEnumerable<Match>> GetByDateRange(DateTime from, DateTime to, TimeSpan clientTimeOffset, string language)
        {
            Guard.Against.OutOfSQLDateRange(to, nameof(to));
            Guard.Against.OutOfSQLDateRange(from, nameof(from));

            var matchByDateSpec = new GetMatchByDateSpecification(
                    int.Parse(Sport.Soccer.Value), from, to, language, appSettings.NumberOfTopMatches);

            var matches = (await matchRepository.ListAsync(matchByDateSpec))
                    .Select(m => m.Match);

            var liveMatches = await GetLive(clientTimeOffset, language);

            return liveMatches
                    .Union(matches)
                    .Select(m => m.ChangeEventDateByTimeZone(clientTimeOffset));
        }
    }
}