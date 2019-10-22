using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Leagues.Models;
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
            await InsertOrUpdateLeagues(message.Leagues, message.Language);
        }

        private async Task InsertOrUpdateLeagues(IEnumerable<League> leagues, string language)
        {
            var internationalLeagues = leagues.Where(league => league.IsInternational);
            await InsertOrUpdateInternationalLeagues(internationalLeagues, language);

            var countryLeagues = leagues.Where(league => !league.IsInternational);
            await InsertOrUpdateCountryLeagues(countryLeagues, language);
        }

        private async Task InsertOrUpdateInternationalLeagues(IEnumerable<League> leagues, string language)
        {
            var command = new InsertOrUpdateInternationalLeaguesCommand(leagues, language);
            await dynamicRepository.ExecuteAsync(command);
        }

        private async Task InsertOrUpdateCountryLeagues(IEnumerable<League> leagues, string language)
        {
            var command = new InsertOrUpdateCountryLeaguesCommand(leagues, language);
            await dynamicRepository.ExecuteAsync(command);
        }
    }
}