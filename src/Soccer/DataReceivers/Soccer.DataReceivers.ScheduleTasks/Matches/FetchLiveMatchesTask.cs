using System;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Logging;
using Hangfire;
using MassTransit;
using Score247.Shared.Enumerations;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders._Shared.Enumerations;
using Soccer.DataProviders.Leagues;
using Soccer.DataProviders.Matches.Services;
using Soccer.DataReceivers.ScheduleTasks._Shared.HealthCheck;
using Soccer.DataReceivers.ScheduleTasks.Leagues;
using Soccer.DataReceivers.ScheduleTasks.Teams;

namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
#pragma warning disable S109 // Magic numbers should not be used

    public interface IFetchLiveMatchesTask
    {
        [DisableConcurrentExecution(timeoutInSeconds: 60)]
        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [Queue("highlive")]
        Task FetchLiveMatches();
    }

#pragma warning restore S109 // Magic numbers should not be used

    public class FetchLiveMatchesTask : IFetchLiveMatchesTask
    {
        private readonly IMatchService matchService;
        private readonly IBus messageBus;
        private readonly ILeagueService internalLeagueService;
        private readonly IHealthCheckService healthCheckService;
        private readonly ILogger logger;

        public FetchLiveMatchesTask(
            IBus messageBus,
            IMatchService matchService,
            Func<DataProviderType, ILeagueService> leagueServiceFactory,
            IHealthCheckService healthCheckService,
            ILogger logger)
        {
            this.messageBus = messageBus;
            this.matchService = matchService;
            internalLeagueService = leagueServiceFactory(DataProviderType.Internal);
            this.healthCheckService = healthCheckService;
            this.logger = logger;
        }

        public async Task FetchLiveMatches()
        {
            await logger.InfoAsync("Start Fetch Live Matches");
            var majorLeagues = await internalLeagueService.GetLeagues(Language.en_US);

            foreach (var language in Enumeration.GetAll<Language>())
            {
                await logger.InfoAsync("In Fetch Live Matches");
                var matches = (await matchService.GetLiveMatches(language))
                     .Where(match => majorLeagues?.Any(league => league.Id == match.League.Id) == true)
                     .ToList();

                await messageBus.Publish<ILiveMatchFetchedMessage>(new LiveMatchFetchedMessage(language, matches));

                var closedMatches = matches.Where(match => match.MatchResult.EventStatus.IsClosed()).ToList();

                if (closedMatches.Count == 0)
                {
                    continue;
                }

                BackgroundJob.Enqueue<IFetchHeadToHeadsTask>(
                    task => task.PublishHeadToHeads(language, closedMatches));

                BackgroundJob.Enqueue<IFetchTimelineTask>(
                    task => task.FetchTimelineEvents(closedMatches, language));

                BackgroundJob.Enqueue<IFetchLeagueStandingsTask>(
                    task => task.FetchClosedMatchesStanding(closedMatches, language));
            }

            await logger.InfoAsync("End Fetch Live Matches");
            await healthCheckService.HeartBeat(nameof(FetchLiveMatches));
        }
    }
}