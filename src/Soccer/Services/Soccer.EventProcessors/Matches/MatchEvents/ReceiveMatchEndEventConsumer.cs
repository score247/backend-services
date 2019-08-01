namespace Soccer.EventProcessors.Matches.MatchEvents
{
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Matches.Events;
    using Soccer.Core.Matches.QueueMessages.MatchEvents;
    using Soccer.Database.Matches.Commands;

    public class ReceiveMatchEndEventConsumer : BaseMatchEventConsumer, IConsumer<IMatchEndEventReceivedMessage>
    {
        private readonly IBus messageBus;
        private readonly IDynamicRepository dynamicRepository;

        public ReceiveMatchEndEventConsumer(IBus messageBus, ICacheService cacheService, IDynamicRepository dynamicRepository)
            : base(cacheService, dynamicRepository)
        {
            this.messageBus = messageBus;
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<IMatchEndEventReceivedMessage> context)
        {
            var matchEvent = context?.Message?.MatchEvent;

            if (await IsTimelineEventNotProcessed(matchEvent))
            {
                await messageBus.Publish<ILiveMatchClosedMessage>(new LiveMatchClosedMessage(matchEvent?.MatchId, matchEvent?.MatchResult));

                await dynamicRepository.ExecuteAsync(new InsertTimelineCommand(matchEvent?.MatchId, matchEvent?.Timeline));
            }
        }
    }
}