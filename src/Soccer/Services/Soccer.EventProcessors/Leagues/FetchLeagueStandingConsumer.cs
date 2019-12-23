using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Leagues.QueueMessages;
using Soccer.Database.Leagues.Commands;

namespace Soccer.EventProcessors.Leagues
{
    public class FetchLeagueStandingConsumer : IConsumer<ILeagueStandingFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public FetchLeagueStandingConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public Task Consume(ConsumeContext<ILeagueStandingFetchedMessage> context)
        {
            var message = context.Message;

            var command = new InsertOrUpdateStandingCommand(
                message.LeagueStanding.League.Id,
                message.LeagueStanding.LeagueSeason.Id,
                message.LeagueStanding.Type.DisplayName,
                message.LeagueStanding,
                message.Language);

            return dynamicRepository.ExecuteAsync(command);
        }
    }
}