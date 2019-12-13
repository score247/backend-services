using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Database.Matches.Commands;

namespace Soccer.EventProcessors.Matches
{
    public class UpdateMatchCoverageConsumer : IConsumer<IMatchUpdatedCoverageInfo>
    {
        private readonly IDynamicRepository dynamicRepository;

        public UpdateMatchCoverageConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public Task Consume(ConsumeContext<IMatchUpdatedCoverageInfo> context)
        {
            var command = new UpdateMatchCoverageCommand(context.Message.MatchId, context.Message.Coverage, context.Message.EventDate);

            return dynamicRepository.ExecuteAsync(command);
        }
    }
}