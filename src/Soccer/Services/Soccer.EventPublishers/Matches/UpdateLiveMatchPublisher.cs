using System.Threading.Tasks;
using Fanex.Logging;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Score247.Shared.Enumerations;
using Soccer.Core.Matches.QueueMessages;
using Soccer.EventPublishers.Hubs;
using Soccer.EventPublishers.Matches.SignalR;

namespace Soccer.EventPublishers.Matches
{
    public class UpdateLiveMatchPublisher : IConsumer<ILiveMatchUpdatedMessage>
    {
        private readonly IHubContext<SoccerHub> hubContext;
        private readonly ILogger logger;

        public UpdateLiveMatchPublisher(IHubContext<SoccerHub> hubContext, ILogger logger)
        {
            this.hubContext = hubContext;
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<ILiveMatchUpdatedMessage> context)
        {
            var message = context.Message;

            if (message != null)
            {
                var newMatches = message.NewMatches;
                var removedMatches = message.RemovedMatches;
                var signalRMessage = JsonConvert.SerializeObject(
                    new LiveMatchSignalRMessage(Sport.Soccer.Value, newMatches, removedMatches));

                await hubContext.Clients.All.SendAsync("LiveMatches", signalRMessage);
                await logger.InfoAsync("Send Live Matches: \r\n" + signalRMessage);
            }
        }
    }
}