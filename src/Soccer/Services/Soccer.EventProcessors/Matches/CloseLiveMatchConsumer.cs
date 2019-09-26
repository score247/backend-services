namespace Soccer.EventProcessors.Matches
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Matches.Events;
    using Soccer.Database.Matches.Commands;

    public class CloseLiveMatchConsumer : IConsumer<ILiveMatchClosedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public CloseLiveMatchConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<ILiveMatchClosedMessage> context)
        {
            var message = context.Message;

            await UpdateMatchResultAndMigrateLiveData(message);
        }

        private async Task UpdateMatchResultAndMigrateLiveData(ILiveMatchClosedMessage message)
        {
            var command = new UpdateMatchResultAndMigrateLiveData(message.MatchId, message.MatchResult);

            await dynamicRepository.ExecuteAsync(command);
        }
    }
}