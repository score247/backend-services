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

    public class OddsComparisonPublisher : IConsumer<IOddsComparisonMessage>
    {
        private const string OddsComparisonName = "OddsComparison";
        private readonly IHubContext<SoccerHub> hubContext;

        public OddsComparisonPublisher(
            IHubContext<SoccerHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<IOddsComparisonMessage> context)
        {
            var matchId = context?.Message?.MatchId;

            if (matchId != null)
            {
                var message = JsonConvert.SerializeObject(
                    new OddsComparisonSignalRMessage(Sport.Soccer.Value, context.Message.MatchId, context.Message.BetTypeOddsList));

                await hubContext.Clients.All.SendAsync(OddsComparisonName, message);
            }
        }
    }
}