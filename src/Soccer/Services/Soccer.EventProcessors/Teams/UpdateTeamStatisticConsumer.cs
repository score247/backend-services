namespace Soccer.EventProcessors.Teams
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using MassTransit;
    using Newtonsoft.Json;
    using Soccer.Core.Teams.QueueMessages;
    using Soccer.Database.Matches.Commands;

    public class UpdateTeamStatisticConsumer : IConsumer<ITeamStatisticUpdatedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ILogger logger;

        public UpdateTeamStatisticConsumer(
            IDynamicRepository dynamicRepository,
            ILogger logger)
        {
            this.dynamicRepository = dynamicRepository;
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<ITeamStatisticUpdatedMessage> context)
        {
            var message = context.Message;

            if (message == null)
            {
                return;
            }

            await logger.InfoAsync($"Update match statistic {message.MatchId}, data: {JsonConvert.SerializeObject(message)}");

            var command = message.IsUpdateOnlyRedCard
                ? (INonQueryCommand)new UpdateLiveMatchTeamRedCardCommand(message.MatchId, message.IsHome, message.TeamStatistic.RedCards, message.TeamStatistic.YellowRedCards)
                : new UpdateLiveMatchTeamStatisticCommand(message.MatchId, message.IsHome, message.TeamStatistic);

            await dynamicRepository.ExecuteAsync(command);
        }
    }
}