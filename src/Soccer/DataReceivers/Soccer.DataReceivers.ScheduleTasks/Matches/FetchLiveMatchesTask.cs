using System.Linq;
using Soccer.DataReceivers.ScheduleTasks.Teams;

namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    using System.Threading.Tasks;
    using Hangfire;
    using MassTransit;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Events;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.DataProviders.Matches.Services;

    public interface IFetchLiveMatchesTask
    {
        [Queue("highlive")]
        Task FetchLiveMatches();
    }

    public class FetchLiveMatchesTask : IFetchLiveMatchesTask
    {
        private readonly IMatchService matchService;
        private readonly IBus messageBus;

        public FetchLiveMatchesTask(
            IBus messageBus,
            IMatchService matchService)
        {
            this.messageBus = messageBus;
            this.matchService = matchService;
        }

        public async Task FetchLiveMatches()
        {
            foreach (var language in Enumeration.GetAll<Language>())
            {
                var matches = await matchService.GetLiveMatches(language);

                await messageBus.Publish<ILiveMatchFetchedMessage>(new LiveMatchFetchedMessage(language, matches));

                var closedMatches = matches.Where(match => match.MatchResult.EventStatus.IsClosed()).ToList();

                if (closedMatches.Count == 0)
                {
                    continue;
                }

                BackgroundJob.Enqueue<IFetchHeadToHeadsTask>(
                    task => task.FetchHeadToHeads(language, closedMatches));

                BackgroundJob.Enqueue<IFetchHeadToHeadsTask>(
                    task => task.FetchTeamResults(language, closedMatches));
            }
        }
    }
}