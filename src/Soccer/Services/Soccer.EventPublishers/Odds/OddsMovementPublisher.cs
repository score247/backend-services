namespace Soccer.EventPublishers.Odds
{
    using System.Threading.Tasks;
    using MassTransit;
    using Microsoft.AspNetCore.SignalR;
    using Newtonsoft.Json;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Odds.Messages;
    using Soccer.EventPublishers.Hubs;
    using Soccer.EventPublishers.Odds.SignalR;

    public class OddsMovementPublisher : IConsumer<IOddsMovementMessage>
    {
        private const string OddsMovementName = "OddsMovement";
        private readonly IHubContext<SoccerHub> hubContext;

        public OddsMovementPublisher(IHubContext<SoccerHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<IOddsMovementMessage> context)
        {
            var matchId = context?.Message?.MatchId;

            if (matchId != null)
            {
                var message = JsonConvert.SerializeObject(
                    new OddsMovementSignalRMessage(Sport.Soccer.Value, context.Message.MatchId, context.Message.OddsEvents));

                await hubContext.Clients.All.SendAsync(OddsMovementName, message);
            }
        }
    }
}