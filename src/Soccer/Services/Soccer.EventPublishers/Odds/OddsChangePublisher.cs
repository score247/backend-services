namespace Soccer.EventPublishers.Odds
{
    using System.Threading.Tasks;
    using Fanex.Logging;
    using MassTransit;
    using Microsoft.AspNetCore.SignalR;
    using Newtonsoft.Json;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Odds.Messages;

    public class OddsChangePublisher : IConsumer<IMatchEventOddsMessage>
    {
        private readonly IHubContext<OddsEventHub> hubContext;
        private readonly ILogger logger;

        public OddsChangePublisher(
            IHubContext<OddsEventHub> hubContext, 
            ILogger logger)
        {
            this.hubContext = hubContext;
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<IMatchEventOddsMessage> context)
        {
            var matchId = context?.Message?.MatchId;

            if (matchId != null)
            {
                await hubContext.Clients.All.SendAsync("MatchOdds", Sport.Soccer.Value, JsonConvert.SerializeObject(context.Message));
                await logger.InfoAsync("Send Match Odds: \r\n" + JsonConvert.SerializeObject(context.Message));
            }
        }
    }
}