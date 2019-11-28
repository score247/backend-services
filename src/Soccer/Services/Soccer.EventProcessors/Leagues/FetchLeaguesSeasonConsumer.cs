using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Leagues.QueueMessages;
using Soccer.Database.Leagues.Commands;

namespace Soccer.EventProcessors.Leagues
{
    public class FetchLeaguesSeasonConsumer : IConsumer<ILeaguesSeasonFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public FetchLeaguesSeasonConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<ILeaguesSeasonFetchedMessage> context)
        {
            var message = context.Message;

            if (message.Leagues?.Any() == false)
            {
                return;
            }

            await InsertLeagueSeasons(message);
            await UpdateLeagueCurrentSeason(message);
        }

        private Task InsertLeagueSeasons(ILeaguesSeasonFetchedMessage message)
        => dynamicRepository.ExecuteAsync(new InsertLeagueSeasonCommand(message.Leagues));

        private Task UpdateLeagueCurrentSeason(ILeaguesSeasonFetchedMessage message)
        => dynamicRepository.ExecuteAsync(new UpdateLeagueCurrentSeasonCommand(message.Leagues));
    }
}
