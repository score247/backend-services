namespace Soccer.API.Modules.Matches
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Ardalis.GuardClauses;
    using Soccer.Core.Domain.Matches.Models;
    using Soccer.Core.Domain.Matches.Repositories;

    public interface IMatchQueryService
    {
        Task<IEnumerable<Match>> GetByDateRange(DateTime from, DateTime to, TimeSpan clientTimeOffset, string language);

        Task<IEnumerable<Match>> GetLive(TimeSpan clientTimeOffset, string language);
    }

    public class MatchQueryService : IMatchQueryService
    {
        private readonly IMatchRepository matchRepository;

        public MatchQueryService(IMatchRepository matchRepository)
        {
            this.matchRepository = matchRepository;
        }

        public async Task<IEnumerable<Match>> GetLive(TimeSpan clientTimeOffset, string language)
        {
            var matches = await matchRepository.GetLive(language);

            return matches.Select(m => m.ChangeEventDateByTimeZone(clientTimeOffset));
        }

        public async Task<IEnumerable<Match>> GetByDateRange(DateTime from, DateTime to, TimeSpan clientTimeOffset, string language)
        {
            Guard.Against.OutOfSQLDateRange(to, nameof(to));
            Guard.Against.OutOfSQLDateRange(from, nameof(from));

            var matches = await matchRepository.GetByDateRange(from.ToUniversalTime(), to.ToUniversalTime(), language);

            return matches.Select(m => m.ChangeEventDateByTimeZone(clientTimeOffset));
        }
    }
}