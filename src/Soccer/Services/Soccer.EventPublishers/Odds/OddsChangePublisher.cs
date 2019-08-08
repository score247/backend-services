namespace Soccer.EventPublishers.Odds
{
    using System.Threading.Tasks;
    using Fanex.Logging;
    using MassTransit;
    using Microsoft.AspNetCore.SignalR;
    using Newtonsoft.Json;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Odds.Messages;

    public class OddsChangePublisher : IConsumer<IOddsChangeOnMatchEventMessage>
    {
        private readonly IHubContext<OddsEventHub> hubContext;
        private readonly ILogger logger;

        public OddsChangePublisher(IHubContext<OddsEventHub> hubContext, ILogger logger)
        {
            this.hubContext = hubContext;
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<IOddsChangeOnMatchEventMessage> context)
        {
            var matchOdds = context?.Message?.MatchOdds;

            if (matchOdds != null)
            {
                await hubContext.Clients.All.SendAsync("MatchOdds", Sport.Soccer.Value, JsonConvert.SerializeObject(matchOdds));
                await logger.InfoAsync("Send Match Odds: \r\n" + JsonConvert.SerializeObject(matchOdds));
            }
        }
    }
}