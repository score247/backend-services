namespace Soccer.EventProcessors.Matches
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Matches.Events;

    public class ReceiveMatchEventsConsumer : IConsumer<IMatchEventsReceivedEvent>
    {
        private readonly IDynamicRepository dynamicRepository;

        public ReceiveMatchEventsConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<IMatchEventsReceivedEvent> context)
        {
            var matchEvent = context.Message.MatchEvent;
        }
    }
}