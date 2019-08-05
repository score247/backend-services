namespace Soccer.EventPublishers.Matches
{
    using System.Threading.Tasks;
    using MassTransit;
    using Microsoft.AspNetCore.SignalR;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Extensions;
    using Soccer.Core.Matches.QueueMessages;
    using Soccer.EventPublishers.Matches.Hubs;

    public class ProcessMatchEventPublisher : IConsumer<IMatchEventProcessedMessage>
    {
        private readonly IHubContext<MatchEventHub> hubContext;

        public ProcessMatchEventPublisher(IHubContext<MatchEventHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<IMatchEventProcessedMessage> context)
        {
            var matchEvent = context?.Message?.MatchEvent;

            if (matchEvent?.Timeline?.IsBasicEvent() == true)
            {
                await hubContext.Clients.All.SendAsync("MatchEvent", Sport.Soccer.Value, matchEvent);
            }
        }
    }
}