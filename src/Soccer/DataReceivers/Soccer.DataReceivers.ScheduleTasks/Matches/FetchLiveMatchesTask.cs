using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using MassTransit;
using Score247.Shared.Enumerations;
using Soccer.Core.Matches.Events;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders._Shared.Enumerations;
using Soccer.DataProviders.Leagues;
using Soccer.DataProviders.Matches.Services;
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

        public FetchLiveMatchesTask(
            IBus messageBus,
            IMatchService matchService,
            Func<DataProviderType, ILeagueService> leagueServiceFactory)
        {
            this.messageBus = messageBus;
            this.matchService = matchService;
            internalLeagueService = leagueServiceFactory(DataProviderType.Internal);
        }

        public async Task FetchLiveMatches()
        {
            var majorLeagues = await internalLeagueService.GetLeagues(Language.en_US);

            foreach (var language in Enumeration.GetAll<Language>())
            {
                var matches = (await matchService.GetLiveMatches(language))
                     .Where(match => majorLeagues?.Any(league => league.Id == match.League.Id) == true);

                await messageBus.Publish<ILiveMatchFetchedMessage>(new LiveMatchFetchedMessage(language, matches));

                var closedMatches = matches.Where(match => match.MatchResult.EventStatus.IsClosed()).ToList();

                if (closedMatches.Count == 0)
                {
                    continue;
                }

                BackgroundJob.Enqueue<IFetchHeadToHeadsTask>(
                    task => task.PublishHeadToHeads(language, closedMatches));

                BackgroundJob.Enqueue<IFetchTimelineTask>(
                    task => task.FetchTimelines(closedMatches, language));
            }
        }
    }
}