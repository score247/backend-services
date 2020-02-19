using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Teams.QueueMessages;
using Soccer.Database.Teams.Commands;

namespace Soccer.EventProcessors.Teams
{
    public class FetchTeamConsumer : IConsumer<ITeamsFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public FetchTeamConsumer(
            IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<ITeamsFetchedMessage> context)
        {
            var message = context.Message;

            if (message == null || !message.Teams.Any())
            {
                return;
            }

            await dynamicRepository.ExecuteAsync(new InsertOrUpdateTeamCommand(message.Teams, message.Language));
        }
    }
}
