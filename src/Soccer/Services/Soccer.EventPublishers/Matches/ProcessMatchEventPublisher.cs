namespace Soccer.EventPublishers.Matches
{
    using System.Threading.Tasks;
    using Fanex.Logging;
    using MassTransit;
    using Microsoft.AspNetCore.SignalR;
    using Newtonsoft.Json;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Extensions;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Matches.QueueMessages;
    using Soccer.EventPublishers.Hubs;

    public class ProcessMatchEventPublisher : IConsumer<IMatchEventProcessedMessage>
    {
        private readonly IHubContext<SoccerHub> hubContext;
        private readonly ILogger logger;

        public ProcessMatchEventPublisher(IHubContext<SoccerHub> hubContext, ILogger logger)
        {
            this.hubContext = hubContext;
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<IMatchEventProcessedMessage> context)
        {
            var matchEvent = context?.Message?.MatchEvent;

            if (matchEvent?.Timeline?.IsBasicEvent() == true)
            {
                var message = JsonConvert.SerializeObject(new MatchEventSignalRMessage(Sport.Soccer.Value, matchEvent));

                const string matchEventName = "MatchEvent";
                await hubContext.Clients.All.SendAsync(matchEventName, message);
                await logger.InfoAsync("Send Match Event: \r\n" + message);
            }
        }
    }

    internal class MatchEventSignalRMessage
    {
        public MatchEventSignalRMessage(byte sportId, MatchEvent matchEvent)
        {
            SportId = sportId;
            MatchEvent = matchEvent;
        }

        public byte SportId { get; }

        public MatchEvent MatchEvent { get; }
    }
}