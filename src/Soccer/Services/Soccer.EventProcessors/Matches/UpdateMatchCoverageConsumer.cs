namespace Soccer.EventProcessors.Matches
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Matches.QueueMessages;
    using Soccer.Database.Matches.Commands;

    public class UpdateMatchCoverageConsumer : IConsumer<IMatchUpdatedCoverageInfo>
    {
        private readonly IDynamicRepository dynamicRepository;

        public UpdateMatchCoverageConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public Task Consume(ConsumeContext<IMatchUpdatedCoverageInfo> context)
        {
            var message = context.Message;

            var command = new UpdateMatchCoverageCommand(message.MatchId, message.Coverage, message.EventDate);

            return dynamicRepository.ExecuteAsync(command);
        }
    }
}