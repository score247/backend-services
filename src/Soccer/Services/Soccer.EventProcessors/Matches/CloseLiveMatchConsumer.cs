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

        public Task Consume(ConsumeContext<ILiveMatchClosedMessage> context)        
            => UpdateMatchResultAndMigrateLiveData(context.Message);

        private Task UpdateMatchResultAndMigrateLiveData(ILiveMatchClosedMessage message)
        {
            var command = new UpdateMatchResultAndMigrateLiveData(message.MatchId, message.MatchResult);

            return dynamicRepository.ExecuteAsync(command);
        }
    }
}