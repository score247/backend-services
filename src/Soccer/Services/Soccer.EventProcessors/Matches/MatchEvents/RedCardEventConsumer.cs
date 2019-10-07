namespace Soccer.EventProcessors.Matches.MatchEvents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Matches.QueueMessages;
    using Soccer.Core.Matches.QueueMessages.MatchEvents;
    using Soccer.Core.Teams.Models;
    using Soccer.Core.Teams.QueueMessages;
    using Soccer.Database.Matches.Criteria;

    public class RedCardEventConsumer : IConsumer<IRedCardEventMessage>
    {
        private readonly IBus messageBus;
        private readonly ICacheService cacheService;
        private readonly IDynamicRepository dynamicRepository;

        private static readonly CacheItemOptions EventCacheOptions = new CacheItemOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(10),
        };

        public RedCardEventConsumer(IBus messageBus, ICacheService cacheService, IDynamicRepository dynamicRepository)
        {
            this.messageBus = messageBus;
            this.cacheService = cacheService;
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<IRedCardEventMessage> context)
        {
            var matchEvent = context?.Message?.MatchEvent;

            if (matchEvent == null)
            {
                return;
            }

            var processedRedCards = await GetProcessedRedCards(matchEvent.MatchId, matchEvent.Timeline.Team);

            var teamStats = new TeamStatistic
            {
                RedCards = processedRedCards.Count(x => x.Type.IsRedCard()),
                YellowRedCards = processedRedCards.Count(x => x.Type.IsYellowRedCard())
            };

            await messageBus.Publish<ITeamStatisticUpdatedMessage>(new TeamStatisticUpdatedMessage(matchEvent.MatchId, matchEvent.Timeline.IsHome, teamStats));
            await messageBus.Publish<IMatchEventProcessedMessage>(new MatchEventProcessedMessage(matchEvent));
        }

        private async Task<IList<TimelineEvent>> GetProcessedRedCards(string matchId, string teamId)
        {
            IList<TimelineEvent> timelineEvents;

            var timelineEventsCacheKey = $"MatchPushEvent_Match_{matchId}";

            timelineEvents = await cacheService.GetOrSetAsync<IList<TimelineEvent>>(
                timelineEventsCacheKey, 
                () => 
                {
                    return (dynamicRepository.Fetch<TimelineEvent>
                                (new GetTimelineEventsCriteria(matchId))).ToList();
                } , 
                EventCacheOptions);

            return timelineEvents.Where(t => t.Team == teamId && (t.Type.IsRedCard() || t.Type.IsYellowRedCard())).ToList();
        }
    }
}