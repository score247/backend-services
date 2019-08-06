namespace Soccer.API.Matches
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Shared.Enumerations;
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
        {
            var match = await dynamicRepository.GetAsync<Match>(new GetMatchByIdCriteria(id, language));

            if (match != null)
            {
                await GetMatchTimelineEvents(id, match);

                GetMatchFunctions(match);
            }

            return match;
        }

        // TODO: Bring to client
        private static void GetMatchFunctions(Match match)
        {
            var functions = new List<MatchFunction>
                {
                    new MatchFunction { Abbreviation = "Odds", Name = "Odds" },
                    new MatchFunction { Abbreviation = "Info", Name = "Match Info" },
                    new MatchFunction { Abbreviation = "Tracker", Name = "Tracker" },
                    new MatchFunction { Abbreviation = "Stats", Name = "Statistics" },
                    new MatchFunction { Abbreviation = "Line-ups", Name = "Line-ups" },
                    new MatchFunction { Abbreviation = "H2H", Name = "Head to Head" },
                    new MatchFunction { Abbreviation = "Table", Name = "Table" },
                    new MatchFunction { Abbreviation = "Social", Name = "Social" },
                    new MatchFunction { Abbreviation = "TV", Name = "TV Schedule" }
                };

            match.Functions = functions;
        }

        private async Task GetMatchTimelineEvents(string id, Match match)
        {
            var timelineEvent = await dynamicRepository.FetchAsync<TimelineEvent>(new GetTimelineEventsCriteria(id));

            match.TimeLines = timelineEvent;
        }
    }
}