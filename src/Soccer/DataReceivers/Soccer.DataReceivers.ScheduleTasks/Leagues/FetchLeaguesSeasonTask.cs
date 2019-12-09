using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using MassTransit;
using Soccer.Core.Leagues.QueueMessages;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders._Shared.Enumerations;
using Soccer.DataProviders.Leagues;
using Soccer.DataReceivers.ScheduleTasks.Shared.Configurations;

namespace Soccer.DataReceivers.ScheduleTasks.Leagues
{
    public interface IFetchLeaguesSeasonTask
    {
        [Queue("low")]
        Task FetchLeaguesSeason();
    }

    public class FetchLeaguesSeasonTask : IFetchLeaguesSeasonTask
    {
        private readonly Func<DataProviderType, ILeagueService> leagueServiceFactory;
        private readonly IBus messageBus;
        private readonly IAppSettings appSettings;

        public FetchLeaguesSeasonTask(IBus messageBus, IAppSettings appSettings, Func<DataProviderType, ILeagueService> leagueServiceFactory)
        {
            this.appSettings = appSettings;
            this.messageBus = messageBus;
            this.leagueServiceFactory = leagueServiceFactory;
        }

        public async Task FetchLeaguesSeason()
        {
            var leagueService = leagueServiceFactory(DataProviderType.SportRadar);
            var batchSize = appSettings.ScheduleTasksSettings.QueueBatchSize;

            //Notes: language not needed
            var soccerLeagues = (await leagueService.GetLeagues(Language.en_US))?.ToList();

            for (var i = 0; i * batchSize < soccerLeagues?.Count; i++)
            {
                var leaguesBatch = soccerLeagues.Skip(i * batchSize).Take(batchSize);

                await messageBus.Publish<ILeaguesSeasonFetchedMessage>(new LeaguesSeasonFetchedMessage(leaguesBatch));
            }
        }
    }
}