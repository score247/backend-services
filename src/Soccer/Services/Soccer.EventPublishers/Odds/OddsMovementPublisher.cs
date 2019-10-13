namespace Soccer.EventPublishers.Odds
{
    using System.Threading.Tasks;
    using Fanex.Logging;
    using MassTransit;
    using Microsoft.AspNetCore.SignalR;
    using Newtonsoft.Json;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Odds.Messages;
    using Soccer.EventPublishers.Hubs;
    using Soccer.EventPublishers.Odds.SignalR;

    public class OddsMovementPublisher : IConsumer<IOddsMovementMessage>
    {
        private readonly IHubContext<SoccerHub> hubContext;
        private readonly ILogger logger;

        public OddsMovementPublisher(
            IHubContext<SoccerHub> hubContext,
            ILogger logger)
        {
            this.hubContext = hubContext;
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<IOddsMovementMessage> context)
        {
            var matchId = context?.Message?.MatchId;

            if (matchId != null)
            {
                var message = JsonConvert.SerializeObject(
                    new OddsMovementSignalRMessage(Sport.Soccer.Value, context.Message.MatchId, context.Message.OddsEvents));
                const string OddsMovementName = "OddsMovement";
                //await hubContext.Clients.All.SendAsync(OddsMovementName, message);
                // await logger.InfoAsync($"Send Odds Movement: {matchId}\r\n" + message);
            }
        }
    }
}