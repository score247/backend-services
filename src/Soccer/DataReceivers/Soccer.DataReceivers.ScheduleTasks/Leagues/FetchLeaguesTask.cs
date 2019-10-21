using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using MassTransit;
using Score247.Shared.Enumerations;
using Soccer.Core.Leagues.QueueMessages;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders.Leagues;
using Soccer.DataReceivers.ScheduleTasks.Shared.Configurations;

namespace Soccer.DataReceivers.ScheduleTasks.Leagues
{
    public interface IFetchLeaguesTask
    {
        [Queue("medium")]
        void FetchLeagues();
    }

    public class FetchLeaguesTask : IFetchLeaguesTask
    {
        private readonly ILeagueService leagueService;
        private readonly IBus messageBus;
        private readonly IAppSettings appSettings;

        public FetchLeaguesTask(IBus messageBus, IAppSettings appSettings, ILeagueService leagueService)
        {
            this.appSettings = appSettings;
            this.messageBus = messageBus;
            this.leagueService = leagueService;
        }

        public void FetchLeagues()
        {
            foreach (var language in Enumeration.GetAll<Language>())
            {
                BackgroundJob.Enqueue(() => FetchLeaguesForLanguage(language));
            }
        }

        public async Task FetchLeaguesForLanguage(Language language)
        {
            var batchSize = appSettings.ScheduleTasksSettings.QueueBatchSize;
            var soccerLeagues = (await leagueService.GetLeagues(language)).ToList();

            for (var i = 0; i * batchSize < soccerLeagues.Count; i++)
            {
                var leaguesBatch = soccerLeagues.Skip(i * batchSize).Take(batchSize);

                await messageBus.Publish<ILeaguesFetchedMessage>(new LeaguesSyncedMessage(leaguesBatch, language.DisplayName));
            }
        }
    }
}