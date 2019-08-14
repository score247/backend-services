namespace Soccer.EventProcessors.Teams
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Teams.QueueMessages;
    using Soccer.Database.Matches.Commands;

    public class UpdateTeamStatisticConsumer : IConsumer<ITeamStatisticUpdatedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public UpdateTeamStatisticConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<ITeamStatisticUpdatedMessage> context)
        {
            var message = context.Message;

            if (message == null)
            {
                return;
            }

            await dynamicRepository.ExecuteAsync(
                new UpdateLiveMatchTeamStatisticCommand(message.MatchId, message.IsHome, message.TeamStatistic));
        }
    }
}