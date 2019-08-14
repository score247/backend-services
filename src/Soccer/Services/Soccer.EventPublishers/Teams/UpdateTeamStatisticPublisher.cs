namespace Soccer.EventPublishers.Teams
{
    using System.Threading.Tasks;
    using Fanex.Logging;
    using MassTransit;
    using Microsoft.AspNetCore.SignalR;
    using Newtonsoft.Json;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Teams.QueueMessages;
    using Soccer.EventPublishers.Teams.Hubs;

    public class UpdateTeamStatisticPublisher : IConsumer<ITeamStatisticUpdatedMessage>
    {
        private readonly IHubContext<TeamStatisticHub> hubContext;
        private readonly ILogger logger;

        public UpdateTeamStatisticPublisher(IHubContext<TeamStatisticHub> hubContext, ILogger logger)
        {
            this.hubContext = hubContext;
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<ITeamStatisticUpdatedMessage> context)
        {
            var message = context?.Message;

            if (message == null)
            {
                return;
            }

            await hubContext.Clients.All.SendAsync("TeamStatistic",
                Sport.Soccer.Value, message.MatchId, message.IsHome, JsonConvert.SerializeObject(message.TeamStatistic));

            await logger.InfoAsync("Send Team Statistic: \r\n" + JsonConvert.SerializeObject(message));
        }
    }
}