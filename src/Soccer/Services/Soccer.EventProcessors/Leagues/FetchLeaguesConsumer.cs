using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Leagues.QueueMessages;
using Soccer.Database.Leagues.Commands;

namespace Soccer.EventProcessors.Leagues
{
    public class FetchLeaguesConsumer : IConsumer<ILeaguesFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public FetchLeaguesConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<ILeaguesFetchedMessage> context)
        {
            var message = context.Message;

            var command = new InsertOrUpdateLeaguesCommand(message.Leagues, message.Language);

            await dynamicRepository.ExecuteAsync(command);
        }
    }
}