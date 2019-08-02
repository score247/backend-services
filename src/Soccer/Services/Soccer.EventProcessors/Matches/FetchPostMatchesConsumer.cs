namespace Soccer.EventProcessors.Matches
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Matches.Events;
    using Soccer.Database.Matches.Commands;

    public class FetchPostMatchesConsumer : IConsumer<IPostMatchFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public FetchPostMatchesConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<IPostMatchFetchedMessage> context)
        {
            var message = context.Message;
            var command = new InsertOrUpdateMatchesCommand(message.Matches, message.Language);

            await dynamicRepository.ExecuteAsync(command);
        }
    }
}