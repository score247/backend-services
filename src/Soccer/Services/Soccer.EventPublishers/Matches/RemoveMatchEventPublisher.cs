using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Soccer.Core.Timelines.QueueMessages;
using Soccer.EventPublishers.Hubs;
using Soccer.EventPublishers.Matches.SignalR;

namespace Soccer.EventPublishers.Matches
{
    public class RemoveMatchEventPublisher : IConsumer<IMatchTimelinesRemovedMessage>
    {
        private const string EventName = "RemovedEvent";
        private readonly IHubContext<SoccerHub> hubContext;

        public RemoveMatchEventPublisher(IHubContext<SoccerHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<IMatchTimelinesRemovedMessage> context)
        {
            if (context?.Message == null)
            {
                return;
            }

            var message = JsonConvert.SerializeObject(new MatchEventRemovedSignalRMessage(context.Message.MatchId, context.Message.TimelineIds));

            await hubContext.Clients.All.SendAsync(EventName, message);
        }
    }
}
