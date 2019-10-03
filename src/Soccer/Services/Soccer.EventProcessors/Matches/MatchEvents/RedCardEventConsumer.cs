namespace Soccer.EventProcessors.Matches.MatchEvents
{
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Matches.QueueMessages;
    using Soccer.Core.Matches.QueueMessages.MatchEvents;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Core.Teams.Models;
    using Soccer.Core.Teams.QueueMessages;
    using Soccer.Database.Matches.Criteria;

    public class RedCardEventConsumer : IConsumer<IRedCardEventMessage>
    {
        private readonly IBus messageBus;
        private readonly ICacheService cacheService;
        private readonly IDynamicRepository dynamicRepository;

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

            var timeline = matchEvent.Timeline;
            var cacheKey = $"{matchEvent.MatchId}_{matchEvent.Timeline.Team}_Statistic";
            TeamStatistic teamStats;

            if (await cacheService.ContainAsync(cacheKey))
            {
                teamStats = await cacheService.GetAsync<TeamStatistic>(cacheKey);

                IncreaseRedCardCount(timeline, teamStats);
            }
            else
            {
                var match = await dynamicRepository.GetAsync<Match>(new GetMatchByIdCriteria(matchEvent.MatchId, Language.en_US));
                teamStats = match?.Teams?.FirstOrDefault(t => t.IsHome == matchEvent.Timeline.IsHome)?.Statistic;

                IncreaseRedCardCount(timeline, teamStats);
            }

            await cacheService.SetAsync(cacheKey, teamStats);
            await messageBus.Publish<ITeamStatisticUpdatedMessage>(new TeamStatisticUpdatedMessage(matchEvent.MatchId, timeline.IsHome, teamStats));
            await messageBus.Publish<IMatchEventProcessedMessage>(new MatchEventProcessedMessage(matchEvent));
        }

        private static void IncreaseRedCardCount(TimelineEvent timeline, TeamStatistic teamStats)
        {
            if (timeline.Type.IsRedCard())
            {
                teamStats.RedCards++;
            }
            else
            {
                teamStats.YellowRedCards++;
            }
        }
    }
}