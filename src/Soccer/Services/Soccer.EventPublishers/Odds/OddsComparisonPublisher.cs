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
                var message = JsonConvert.SerializeObject(new OddsComparisonSignalRMessage(Sport.Soccer.Value, context.Message));
                const string OddsComparisonName = "OddsComparison";
                await hubContext.Clients.All.SendAsync(OddsComparisonName, message);
                await logger.InfoAsync("Send Odds Comparison: \r\n" + message);
            }
        }
    }

    internal class OddsComparisonSignalRMessage
    {
        public OddsComparisonSignalRMessage(byte sportId, IOddsComparisonMessage oddsComparison)
        {
            SportId = sportId;
            OddsComparison = oddsComparison;
        }

        public byte SportId { get; }

        public IOddsComparisonMessage OddsComparison { get; }
    }
}
