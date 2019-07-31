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

            await UpdateMatchResult(message);

            await RemoveLiveMatch(message);
        }

        private async Task UpdateMatchResult(ILiveMatchClosedMessage message)
        {
            var command = new UpdateMatchResultCommand(message.MatchId, message.MatchResult, message.Language);

            await dynamicRepository.ExecuteAsync(command);
        }

        private async Task RemoveLiveMatch(ILiveMatchClosedMessage message)
        {
            var command = new RemoveLiveMatchCommand(message.MatchId);

            await dynamicRepository.ExecuteAsync(command);
        }
    }
}