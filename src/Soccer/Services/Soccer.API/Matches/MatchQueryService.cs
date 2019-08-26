namespace Soccer.API.Matches
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Soccer.API.Matches.Models;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Database.Matches.Criteria;

    public interface IMatchQueryService
    {
        Task<IEnumerable<MatchSummary>> GetByDateRange(DateTime from, DateTime to, Language language);

        Task<MatchInfo> GetMatchInfo(string id, Language language);

        Task<IEnumerable<Match>> GetLive(TimeSpan clientTimeOffset, Language language);
    }

    public class MatchQueryService : IMatchQueryService
    {
        private readonly IDynamicRepository dynamicRepository;

        public MatchQueryService(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task<IEnumerable<Match>> GetLive(TimeSpan clientTimeOffset, Language language)
            => await dynamicRepository.FetchAsync<Match>(new GetLiveMatchesCriteria(language));

        public async Task<IEnumerable<MatchSummary>> GetByDateRange(DateTime from, DateTime to, Language language)
        {
            var matches = await dynamicRepository.FetchAsync<Match>(new GetMatchesByDateRangeCriteria(from, to, language));

            return matches.Select(m => new MatchSummary(m));
        }

        public async Task<MatchInfo> GetMatchInfo(string id, Language language)
        {
            var match = await dynamicRepository.GetAsync<Match>(new GetMatchByIdCriteria(id, language));

            if (match != null)
            {
                var timelineEvents = await dynamicRepository.FetchAsync<TimelineEvent>(new GetTimelineEventsCriteria(id));

                return new MatchInfo(match, timelineEvents);
            }

            return null;
        }
    }
}