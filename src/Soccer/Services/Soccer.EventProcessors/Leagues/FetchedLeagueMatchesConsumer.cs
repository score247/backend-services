using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Leagues.QueueMessages;
using Soccer.Database.Leagues.Commands;

namespace Soccer.EventProcessors.Leagues
{
    public class FetchedLeagueMatchesConsumer : IConsumer<ILeagueMatchesFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public FetchedLeagueMatchesConsumer(IDynamicRepository dynamicRepository) 
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<ILeagueMatchesFetchedMessage> context)
        {
            var message = context.Message;

            if (message.LeagueSeasons?.Any() == false)
            {
                return;
            }

            var command = new UpdateFetchedLeagueSeasonCommand(message.LeagueSeasons);

            await dynamicRepository.ExecuteAsync(command);
        }
    }
}
