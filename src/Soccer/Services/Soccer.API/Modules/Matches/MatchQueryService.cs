namespace Soccer.API.Modules.Matches
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Ardalis.GuardClauses;
    using Fanex.Data.Repository;
    using Soccer.Core.Domain.Matches.Models;
    using Soccer.Core.Domain.Matches.Queries;

    public interface IMatchQueryService
    {
        Task<IEnumerable<Match>> GetByDateRange(DateTime from, DateTime to, TimeSpan clientTimeOffset, string language);

        Task<IEnumerable<Match>> GetLive(TimeSpan clientTimeOffset, string language);
    }

    public class MatchQueryService : IMatchQueryService
    {
        private readonly IDynamicRepository dynamicRepository;

        public MatchQueryService(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task<IEnumerable<Match>> GetLive(TimeSpan clientTimeOffset, string language)
        {
            var query = new GetLiveMatchesQuery(language);
            var matches = await dynamicRepository.FetchAsync<Match>(query);

            // Remove convert to client timezone, remember to move this business to front-end
            return matches;
        }

        public async Task<IEnumerable<Match>> GetByDateRange(DateTime from, DateTime to, TimeSpan clientTimeOffset, string language)
        {
            Guard.Against.OutOfSQLDateRange(to, nameof(to));
            Guard.Against.OutOfSQLDateRange(from, nameof(from));

            var query = new GetMatchesByDateRangeQuery(from, to, language);
            var matches = await dynamicRepository.FetchAsync<Match>(query);

            // Remove convert to client timezone, remember to move this business to front-end
            return matches;
        }
    }
}