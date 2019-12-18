using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Leagues.QueueMessages;
using Soccer.Database.Leagues.Commands;

namespace Soccer.EventProcessors.Leagues
{
    public class FetchedLeagueGroupConsumer : IConsumer<ILeagueGroupFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public FetchedLeagueGroupConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public Task Consume(ConsumeContext<ILeagueGroupFetchedMessage> context)
        {
            var message = context.Message;
          
            var command = new InsertOrUpdateLeagueGroupCommand(message.LeagueId, message.LeagueGroupName, message.Language);

            return dynamicRepository.ExecuteAsync(command);
        }
    }
}
