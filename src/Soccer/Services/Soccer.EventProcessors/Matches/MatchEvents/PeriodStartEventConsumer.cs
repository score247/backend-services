namespace Soccer.EventProcessors.Matches.MatchEvents
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Matches.QueueMessages;
    using Soccer.Core.Matches.QueueMessages.MatchEvents;
    using Soccer.Database.Matches.Commands;

    public class PeriodStartEventConsumer : IConsumer<IPeriodStartEventMessage>
    {
        private readonly IBus messageBus;
        private readonly IDynamicRepository dynamicRepository;

        public PeriodStartEventConsumer(IBus messageBus, IDynamicRepository dynamicRepository)
        {
            this.messageBus = messageBus;
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<IPeriodStartEventMessage> context)
        {
            var matchEvent = context?.Message?.MatchEvent;
            var command = new UpdateLiveMatchCurrentPeriodStartTimeCommand(
                    matchEvent?.MatchId, matchEvent?.Timeline?.Time ?? DateTime.Now);
            await dynamicRepository.ExecuteAsync(command);

            await messageBus.Publish<IMatchEventProcessedMessage>(new MatchEventProcessedMessage(matchEvent));
        }
    }
}