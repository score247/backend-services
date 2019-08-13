namespace Soccer.EventProcessors.Matches.MatchEvents
{
    using System.Threading.Tasks;
    using MassTransit;
    using Soccer.Core.Matches.Events;
    using Soccer.Core.Matches.QueueMessages;
    using Soccer.Core.Matches.QueueMessages.MatchEvents;

    public class MatchEndEventConsumer : IConsumer<IMatchEndEventMessage>
    {
        private readonly IBus messageBus;

        public MatchEndEventConsumer(IBus messageBus)
        {
            this.messageBus = messageBus;
        }

        public async Task Consume(ConsumeContext<IMatchEndEventMessage> context)
        {
            var matchEvent = context?.Message?.MatchEvent;

            await messageBus.Publish<IMatchEventProcessedMessage>(new MatchEventProcessedMessage(matchEvent));
            await messageBus.Publish<ILiveMatchClosedMessage>(new LiveMatchClosedMessage(matchEvent?.MatchId, matchEvent?.MatchResult));
        }
    }
}