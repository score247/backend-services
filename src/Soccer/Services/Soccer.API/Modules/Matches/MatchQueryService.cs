namespace Soccer.API.Modules.Matches
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Soccer.Core.Domain.Matches;
    using Soccer.Core.Domain.Matches.Models;
    using Soccer.Core.Domain.Matches.Specifications;

    public interface IMatchQueryService
    {
        Task<IEnumerable<Match>> GetByDateRange(int sportId, DateTime from, DateTime to, TimeSpan clientTimeZone, string language);

        Task<IEnumerable<Match>> GetLive(int sportId, string language);
    }

    public class MatchQueryService : IMatchQueryService
    {
        private readonly IMatchRepository matchRepository;

        public MatchQueryService(IMatchRepository matchRepository)
        {
            this.matchRepository = matchRepository;
        }

        public Task<IEnumerable<Match>> GetLive(int sportId, string language)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Match>> GetByDateRange(int sportId, DateTime from, DateTime to, TimeSpan clientTimeZone, string language)
        {
            var matchByDateSpec = new MatchByDateSpecification(sportId, from, to, language);
            var matchEntities = await matchRepository.ListAsync(matchByDateSpec);
            var matches = matchEntities
                    .Select(m => m.Match)
                    .Select(m => m.ChangeEventDateByTimeZone(clientTimeZone));

            return matches;
        }
    }
}