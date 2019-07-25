namespace Soccer.EventProcessors.Matches
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Domain.Matches.Commands;
    using Soccer.Core.Domain.Matches.Events;

    public class UpdatePostMatchesConsumer : IConsumer<IPostMatchUpdatedEvent>
    {
        private readonly IDynamicRepository dynamicRepository;

        public UpdatePostMatchesConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<IPostMatchUpdatedEvent> context)
        {
            var message = context.Message;
            var command = new InsertOrUpdateMatchesCommand(message.Matches, message.Language);

            await dynamicRepository.ExecuteAsync(command);
        }
    }
}