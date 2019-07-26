namespace Soccer.API.Matches
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Soccer.Core._Shared.Enumerations;
    using Soccer.Core.Matches.Models;
    using Soccer.Database.Matches.Criteria;

    public interface IMatchQueryService
    {
        Task<IEnumerable<Match>> GetByDateRange(DateTime from, DateTime to, TimeSpan clientTimeOffset, Language language);

        Task<Match> GetMatch(string id, Language language);

        Task<IEnumerable<Match>> GetLive(TimeSpan clientTimeOffset, Language language);
    }

    public class MatchQueryService : IMatchQueryService
    {
        private readonly IDynamicRepository dynamicRepository;

        public MatchQueryService(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        // TODO: Remove convert to client timezone, remember to move this business to front-end
        public async Task<IEnumerable<Match>> GetLive(TimeSpan clientTimeOffset, Language language)
            => await dynamicRepository.FetchAsync<Match>(new GetLiveMatchesCriteria(language));

        // TODO: Remove convert to client timezone, remember to move this business to front-end
        public async Task<IEnumerable<Match>> GetByDateRange(DateTime from, DateTime to, TimeSpan clientTimeOffset, Language language)
            => await dynamicRepository.FetchAsync<Match>(new GetMatchesByDateRangeCriteria(from, to, language));

        // TODO: Remove convert to client timezone, remember to move this business to front-end
        public async Task<Match> GetMatch(string id, Language language)
            => await dynamicRepository.GetAsync<Match>(new GetMatchByIdCriteria(id, language));
    }
}