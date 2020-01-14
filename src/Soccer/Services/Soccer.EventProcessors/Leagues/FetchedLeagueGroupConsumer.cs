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

        public async Task Consume(ConsumeContext<ILeagueGroupFetchedMessage> context)
        {
            var message = context.Message;

            if (string.IsNullOrWhiteSpace(message.LeagueGroupStage.GroupStageName))
            {
                return;
            }

            await Task.WhenAll(
                dynamicRepository.ExecuteAsync(
                    new InsertOrUpdateLeagueGroupCommand(message.LeagueGroupStage, message.Language)),
                dynamicRepository.ExecuteAsync(
                    new EnableHasGroupsCommand(message.LeagueGroupStage.LeagueId)));
        }
    }
}