namespace Soccer.EventProcessors.Matches
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Matches.Events;
    using Soccer.Database.Matches.Commands;

    public class UpdateLiveMatchResultConsumer : IConsumer<ILiveMatchResultUpdatedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public UpdateLiveMatchResultConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<ILiveMatchResultUpdatedMessage> context)
        {
            var message = context.Message;

            var command = new UpdateLiveMatchResultCommand(message.MatchId, message.MatchResult);

            await dynamicRepository.ExecuteAsync(command);
        }
    }
}