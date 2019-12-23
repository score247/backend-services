using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Leagues.Models;
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

            var leagueGroupState = new LeagueGroupState(
                message.LeagueId,
                message.LeagueSeasonId,
                message.LeagueGroupName,
                message.LeagueRound,
                message.Language.DisplayName);

            var command = new InsertOrUpdateLeagueGroupCommand(leagueGroupState);

            return dynamicRepository.ExecuteAsync(command);
        }
    }
}