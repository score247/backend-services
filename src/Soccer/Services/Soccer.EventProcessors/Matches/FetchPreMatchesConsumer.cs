namespace Soccer.EventProcessors.Matches
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Domain.Matches.Events;
    using Soccer.Database.Matches.Commands;

    public class FetchPreMatchesConsumer : IConsumer<IPreMatchesFetchedEvent>
    {
        private readonly IDynamicRepository dynamicRepository;

        public FetchPreMatchesConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<IPreMatchesFetchedEvent> context)
        {
            var message = context.Message;
            var command = new InsertOrUpdateMatchesCommand(message.Matches, message.Language);

            await dynamicRepository.ExecuteAsync(command);
        }
    }
}