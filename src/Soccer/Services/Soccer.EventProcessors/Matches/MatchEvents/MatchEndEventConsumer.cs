namespace Soccer.EventProcessors.Matches.MatchEvents
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Matches.Events;
    using Soccer.Core.Matches.QueueMessages.MatchEvents;
    using Soccer.Database.Matches.Commands;

    public class MatchEndEventConsumer : IConsumer<IMatchEndEventMessage>
    {
        private readonly IBus messageBus;
        private readonly IDynamicRepository dynamicRepository;

        public MatchEndEventConsumer(IBus messageBus, IDynamicRepository dynamicRepository)
        {
            this.messageBus = messageBus;
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<IMatchEndEventMessage> context)
        {
            var matchEvent = context?.Message?.MatchEvent;

            await messageBus.Publish<ILiveMatchClosedMessage>(new LiveMatchClosedMessage(matchEvent?.MatchId, matchEvent?.MatchResult));

            await dynamicRepository.ExecuteAsync(new InsertTimelineCommand(matchEvent?.MatchId, matchEvent?.Timeline));
        }
    }
}