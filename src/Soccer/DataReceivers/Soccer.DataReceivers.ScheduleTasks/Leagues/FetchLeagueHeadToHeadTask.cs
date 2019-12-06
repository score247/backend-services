using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders._Shared.Enumerations;
using Soccer.DataProviders.Leagues;
using Soccer.DataReceivers.ScheduleTasks.Shared.Configurations;
using Soccer.DataReceivers.ScheduleTasks.Teams;

namespace Soccer.DataReceivers.ScheduleTasks.Leagues
{
    public interface IFetchLeagueHeadToHeadTask
    {
        [Queue("low")]
        Task FetchHeadToHeadOfAllLeague();

        [Queue("low")]
        Task FetchHeadToHeadOfLeagues(IList<League> batchOfLeagues);
    }

    public class FetchLeagueHeadToHeadTask : IFetchLeagueHeadToHeadTask
    {
        private const int FetchLeagueScheduleDelay = 1;
        private const int FetchHeadToHeadDelay = 3;

        private readonly Func<DataProviderType, ILeagueService> leagueServiceFactory;
        private readonly IAppSettings appSettings;
        private readonly IBackgroundJobClient jobClient;
        private readonly ILeagueScheduleService leagueScheduleService;

        public FetchLeagueHeadToHeadTask(
            IAppSettings appSettings,
            ILeagueScheduleService leagueScheduleService,
            Func<DataProviderType, ILeagueService> leagueServiceFactory,
            IBackgroundJobClient jobClient)
        {
            this.appSettings = appSettings;
            this.leagueScheduleService = leagueScheduleService;
            this.leagueServiceFactory = leagueServiceFactory;
            this.jobClient = jobClient;
        }

        public async Task FetchHeadToHeadOfAllLeague()
        {
            var leagueService = leagueServiceFactory(DataProviderType.Internal);

            // Note: fetch h2h for en_US only
            var majorLeagues = await leagueService.GetLeagues(Language.en_US);

            for (var i = 0; i * appSettings.ScheduleTasksSettings.QueueBatchSize < majorLeagues.Count(); i++)
            {
                var batchOfLeagues = majorLeagues
                    .Skip(i * appSettings.ScheduleTasksSettings.QueueBatchSize)
                    .Take(appSettings.ScheduleTasksSettings.QueueBatchSize)
                    .ToList();

                var delayTimespan = TimeSpan.FromMinutes(FetchLeagueScheduleDelay + i);

                jobClient.Schedule<IFetchLeagueHeadToHeadTask>(t => t.FetchHeadToHeadOfLeagues(batchOfLeagues), delayTimespan);
            }
        }

        public async Task FetchHeadToHeadOfLeagues(IList<League> batchOfLeagues)
        {
            foreach (var league in batchOfLeagues)
            {
                var matches = await leagueScheduleService.GetLeagueMatches(league.Region, league.Id, Language.en_US);

                for (var i = 0; i * appSettings.ScheduleTasksSettings.QueueBatchSize < matches.Count(); i++)
                {
                    var batchOfMatches = matches
                        .Skip(i * appSettings.ScheduleTasksSettings.QueueBatchSize)
                        .Take(appSettings.ScheduleTasksSettings.QueueBatchSize)
                        .ToList();

                    var delayTimespan = TimeSpan.FromMinutes(FetchHeadToHeadDelay + i);

                    jobClient.Schedule<IFetchHeadToHeadsTask>(t => t.FetchHeadToHeads(Language.en_US, batchOfMatches), delayTimespan);
                }
            }
        }
    }
}
