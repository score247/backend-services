namespace Soccer.EventPublishers.Teams
{
    using System.Threading.Tasks;
    using Fanex.Logging;
    using MassTransit;
    using Microsoft.AspNetCore.SignalR;
    using Newtonsoft.Json;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Teams.QueueMessages;
    using Soccer.EventPublishers.Hubs;

    public class UpdateTeamStatisticPublisher : IConsumer<ITeamStatisticUpdatedMessage>
    {
        private readonly IHubContext<SoccerHub> hubContext;
        private readonly ILogger logger;

        public UpdateTeamStatisticPublisher(IHubContext<SoccerHub> hubContext, ILogger logger)
        {
            this.hubContext = hubContext;
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<ITeamStatisticUpdatedMessage> context)
        {
            if (context?.Message == null)
            {
                return;
            }

            var message = JsonConvert.SerializeObject(new TeamStatisticSignalRMessage(
                Sport.Soccer.Value,
                context.Message));

            const string TeamStatisticName = "TeamStatistic";
            await hubContext.Clients.All.SendAsync(TeamStatisticName, message);

            await logger.InfoAsync("Send Team Statistic: \r\n" + message);
        }
    }

    internal class TeamStatisticSignalRMessage
    {
        public TeamStatisticSignalRMessage(
            byte sportId,
            ITeamStatisticUpdatedMessage teamStatistic)
        {
            SportId = sportId;
            TeamStatistic = teamStatistic;
        }

        public byte SportId { get; }

        public ITeamStatisticUpdatedMessage TeamStatistic { get; }
    }
}