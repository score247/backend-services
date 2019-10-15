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

    public class OddsComparisonPublisher : IConsumer<IOddsComparisonMessage>
    {
        private readonly IHubContext<SoccerHub> hubContext;
        private readonly ILogger logger;

        public OddsComparisonPublisher(
            IHubContext<SoccerHub> hubContext,
            ILogger logger)
        {
            this.hubContext = hubContext;
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<IOddsComparisonMessage> context)
        {
            var matchId = context?.Message?.MatchId;

            if (matchId != null)
            {
                var message = JsonConvert.SerializeObject(
                    new OddsComparisonSignalRMessage(Sport.Soccer.Value, context.Message.MatchId, context.Message.BetTypeOddsList));
                //await hubContext.Clients.All.SendAsync(OddsComparisonName, message);
                // await logger.InfoAsync($"Send Odds Comparison: {matchId}\r\n" + message);
            }
        }
    }
}