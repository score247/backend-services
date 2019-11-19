using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using MassTransit;
using Score247.Shared.Enumerations;
using Soccer.Core.Leagues.QueueMessages;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders._Shared.Enumerations;
using Soccer.DataProviders.Leagues;
using Soccer.DataReceivers.ScheduleTasks.Shared.Configurations;

namespace Soccer.DataReceivers.ScheduleTasks.Leagues
{
    public interface IFetchLeaguesTask
    {
        [Queue("low")]
        void FetchLeagues();

        [Queue("low")]
        Task FetchLeaguesForLanguage(Language language);
    }

    public class FetchLeaguesTask : IFetchLeaguesTask
    {
        private readonly Func<DataProviderType, ILeagueService> leagueServiceFactory;
        private readonly IBus messageBus;
        private readonly IAppSettings appSettings;

        public FetchLeaguesTask(IBus messageBus, IAppSettings appSettings, Func<DataProviderType, ILeagueService> leagueServiceFactory)
        {
            this.appSettings = appSettings;
            this.messageBus = messageBus;
            this.leagueServiceFactory = leagueServiceFactory;
        }

        public void FetchLeagues()
        {
            foreach (var language in Enumeration.GetAll<Language>())
            {
                BackgroundJob.Enqueue<IFetchLeaguesTask>(t => t.FetchLeaguesForLanguage(language));
            }
        }

        public async Task FetchLeaguesForLanguage(Language language)
        {
            var leagueService = leagueServiceFactory(DataProviderType.SportRadar);

            var batchSize = appSettings.ScheduleTasksSettings.QueueBatchSize;
            var soccerLeagues = (await leagueService.GetLeagues(language)).ToList();

            for (var i = 0; i * batchSize < soccerLeagues.Count; i++)
            {
                var leaguesBatch = soccerLeagues.Skip(i * batchSize).Take(batchSize);

                await messageBus.Publish<ILeaguesFetchedMessage>(new LeaguesFetchedMessage(leaguesBatch, language.DisplayName));
            }
        }
    }
}