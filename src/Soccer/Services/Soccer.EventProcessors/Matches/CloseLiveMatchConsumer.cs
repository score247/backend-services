using Soccer.Core.Matches.QueueMessages;

namespace Soccer.EventProcessors.Matches
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Database.Matches.Commands;

    public class CloseLiveMatchConsumer : IConsumer<ILiveMatchClosedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public CloseLiveMatchConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public Task Consume(ConsumeContext<ILiveMatchClosedMessage> context)
            => UpdateMatchResult(context.Message);

        private Task UpdateMatchResult(ILiveMatchClosedMessage message)
        {
            var command = new UpdateLiveMatchResultCommand(message.MatchId, message.MatchResult);

            return dynamicRepository.ExecuteAsync(command);
        }
    }
}