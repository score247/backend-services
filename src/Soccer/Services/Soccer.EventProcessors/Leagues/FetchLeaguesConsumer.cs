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

            if (message.Leagues?.Any() == false)
            {
                return;
            }

            await InsertOrUpdateLeagues(message.Leagues, message.Language);
            await InsertLeagueSeasons(message.Leagues, message.Language);
        }

        private async Task InsertOrUpdateLeagues(IEnumerable<League> leagues, string language)
        {
            var internationalLeagues = leagues.Where(league => league.IsInternational);

            if (internationalLeagues?.Any() == true)
            {
                await InsertOrUpdateInternationalLeagues(internationalLeagues, language);
            }

            var countryLeagues = leagues.Where(league => !league.IsInternational);

            if (countryLeagues?.Any() == true)
            {
                await InsertOrUpdateCountryLeagues(countryLeagues, language);
            }
        }

        private Task InsertOrUpdateInternationalLeagues(IEnumerable<League> leagues, string language)
        {
            var command = new InsertOrUpdateInternationalLeaguesCommand(leagues, language);
            return dynamicRepository.ExecuteAsync(command);
        }

        private Task InsertOrUpdateCountryLeagues(IEnumerable<League> leagues, string language)
        {
            var command = new InsertOrUpdateCountryLeaguesCommand(leagues, language);
            return dynamicRepository.ExecuteAsync(command);
        }

        private Task InsertLeagueSeasons(IEnumerable<League> leagues, string language)
        {
            var command = new InsertLeagueSeasonCommand(leagues, language);

            return dynamicRepository.ExecuteAsync(command);
        }
    }
}