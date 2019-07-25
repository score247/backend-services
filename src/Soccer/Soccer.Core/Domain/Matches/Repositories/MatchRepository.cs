namespace Soccer.Core.Domain.Matches.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Newtonsoft.Json;
    using Soccer.Core.Domain.Matches.Models;
    using Soccer.Core.Domain.Matches.Repositories.Commands;
    using Soccer.Core.Domain.Matches.Repositories.Queries;

    public interface IMatchRepository
    {
        Task<IEnumerable<Match>> GetByDateRange(DateTime from, DateTime to, string language);

        Task<IEnumerable<Match>> GetLive(string language);

        Task InsertOrUpdateMatches(IReadOnlyList<Match> matches, string language);
    }

    public class MatchRepository : IMatchRepository
    {
        private readonly IDynamicRepository dynamicRepository;

        public MatchRepository(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task<IEnumerable<Match>> GetByDateRange(DateTime from, DateTime to, string language)
        {
            var query = new GetMatchesByDateRangeQuery(from, to, language);
            var responseData = await dynamicRepository.FetchAsync<string>(query);

            return responseData.Select(JsonConvert.DeserializeObject<Match>);
        }

        public async Task<IEnumerable<Match>> GetLive(string language)
        {
            var query = new GetLiveMatchesQuery(language);
            var responseData = await dynamicRepository.FetchAsync<string>(query);

            return responseData.Select(JsonConvert.DeserializeObject<Match>);
        }

        public async Task InsertOrUpdateMatches(IReadOnlyList<Match> matches, string language)
        {
            var command = new InsertOrUpdateMatchesCommand(matches, language);

            await dynamicRepository.ExecuteAsync(command);
        }
    }
}