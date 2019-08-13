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
    using Soccer.Database.Matches.Commands;
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

            var cacheKey = $"RedCards_{matchEvent.MatchId}_{matchEvent.Timeline.Team}";
            var redCards = 0;

            if (await cacheService.ContainAsync(cacheKey))
            {
                var currentRedCards = await cacheService.GetAsync<int>(cacheKey);
                redCards = currentRedCards + 1;
            }
            else
            {
                var match = await dynamicRepository.GetAsync<Match>(new GetMatchByIdCriteria(matchEvent.MatchId, Language.en_US));
                var currentTeam = match?.Teams?.FirstOrDefault(t => t.IsHome == matchEvent.Timeline.IsHome);

                if (currentTeam != null)
                {
                    redCards = currentTeam.Statistic.RedCards + 1;
                }
            }

            await cacheService.SetAsync(cacheKey, redCards);
            await dynamicRepository.ExecuteAsync(
                new UpdateLiveMatchTeamRedCardsCommand(matchEvent.MatchId, matchEvent.Timeline.IsHome, redCards));

            await messageBus.Publish<IMatchEventProcessedMessage>(new MatchEventProcessedMessage(matchEvent));
        }
    }
}