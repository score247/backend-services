namespace Soccer.EventProcessors.Matches
{
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Matches.Events;
    using Soccer.Database.Matches.Commands;
    using System.Threading.Tasks;

    public class UpdateLiveMatchConsumer : IConsumer<ILiveMatchUpdatedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public UpdateLiveMatchConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<ILiveMatchUpdatedMessage> context)
        {
            var message = context.Message;

            var command = new UpdateLiveMatchResultCommand(message.MatchId, message.MatchResult);

            await dynamicRepository.ExecuteAsync(command);
        }
    }
}
