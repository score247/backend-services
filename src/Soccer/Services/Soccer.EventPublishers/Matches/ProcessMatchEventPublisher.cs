namespace Soccer.EventPublishers.Matches
{
    using System.Threading.Tasks;
    using Fanex.Logging;
    using MassTransit;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Extensions;
    using Soccer.Core.Matches.QueueMessages;
    using Soccer.EventPublishers.Matches.Hubs;

    public class ProcessMatchEventPublisher : IConsumer<IMatchEventProcessedMessage>
    {
        private readonly IHubContext<MatchEventHub> hubContext;
        private readonly ILogger logger;

        public ProcessMatchEventPublisher(IHubContext<MatchEventHub> hubContext, ILogger logger)
        {
            this.hubContext = hubContext;
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<IMatchEventProcessedMessage> context)
        {
            var matchEvent = context?.Message?.MatchEvent;

            if (matchEvent?.Timeline?.IsBasicEvent() == true)
            {
                await hubContext.Clients.All.SendAsync("MatchEvent", Sport.Soccer.Value, JsonConvert.SerializeObject(matchEvent));
                await logger.InfoAsync("Send Match Event: \r\n" + JsonConvert.SerializeObject(matchEvent));
            }
        }
    }
}