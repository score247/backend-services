using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Teams.QueueMessages;
using Soccer.Database.Teams;

namespace Soccer.EventProcessors.Teams
{
    public class FetchHeadToHeadConsumer : IConsumer<IHeadToHeadFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public FetchHeadToHeadConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<IHeadToHeadFetchedMessage> context)
        {
            var message = context?.Message;

            if (message != null)
            {
                await dynamicRepository.ExecuteAsync(new InsertOrUpdateHeadToHeadCommand(
                    message.HomeTeamId,
                    message.AwayTeamId,
                    message.HeadToHeadMatch,
                    message.Language));
            }
        }
    }
}