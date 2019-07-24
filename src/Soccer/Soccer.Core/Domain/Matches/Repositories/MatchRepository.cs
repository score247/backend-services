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
    using Soccer.Core.Domain.Matches.Repositories.Criterias;
    using Soccer.Core.Domain.Matches.Repositories.DbModels;

    public interface IMatchRepository
    {
        Task<IEnumerable<Match>> GetByDateRange(DateTime from, DateTime to, string language);

        Task InsertOrUpdatePreMatches(IReadOnlyList<Match> matches, string language);
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
            var criteria = new GetMatchesByDateRangeCriteria(from, to, language);
            var responseData = await dynamicRepository.FetchAsync<string>(criteria);

            return responseData.Select(JsonConvert.DeserializeObject<Match>);
        }

        public async Task InsertOrUpdatePreMatches(IReadOnlyList<Match> matches, string language)
        {
            var matchEntities = matches.Select(m => new MatchEntity(m, language));
            var command = new InsertOrUpdatePreMatchesCommand(matchEntities);

            await dynamicRepository.ExecuteAsync(command);
        }
    }
}