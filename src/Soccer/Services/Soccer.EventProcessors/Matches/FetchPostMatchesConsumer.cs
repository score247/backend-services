namespace Soccer.EventProcessors.Matches
{
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Matches.QueueMessages;
    using Soccer.Database.Matches.Commands;
    using System.Threading.Tasks;

    public class FetchPostMatchesConsumer : IConsumer<IPostMatchUpdatedResultMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public FetchPostMatchesConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<IPostMatchUpdatedResultMessage> context)
        {
            var message = context.Message;
            var command = new UpdatePostMatchResultCommand(message.MatchId, message.Language, message.Result);

            await dynamicRepository.ExecuteAsync(command);
        }
    }
}