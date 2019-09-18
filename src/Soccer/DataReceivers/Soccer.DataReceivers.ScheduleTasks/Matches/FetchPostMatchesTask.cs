namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    using Hangfire;
    using MassTransit;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Events;
    using Soccer.Core.Matches.QueueMessages;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.DataProviders.Matches.Services;
    using System;
    using System.Threading.Tasks;

    public interface IFetchPostMatchesTask
    {
        void FetchPostMatches(int dateSpan);

        Task FetchPostMatchesForDate(DateTime date, Language language);
    }

    public class FetchPostMatchesTask : IFetchPostMatchesTask
    {
        private readonly IMatchService matchService;
        private readonly IBus messageBus;

        public FetchPostMatchesTask(IBus messageBus, IMatchService matchService)
        {            
            this.messageBus = messageBus;
            this.matchService = matchService;
        }

        public void FetchPostMatches(int dateSpan)
        {
            var from = DateTime.UtcNow.AddDays(-dateSpan);
            var to = DateTime.UtcNow;

            foreach (var language in Enumeration.GetAll<Language>())
            {
                for (var date = from; date.Date <= to; date = date.AddDays(1))
                {
                    BackgroundJob.Enqueue(() => FetchPostMatchesForDate(date, language));
                }
            }
        }

        public async Task FetchPostMatchesForDate(DateTime date, Language language)
        {           
            var matches = await matchService.GetPostMatches(date, language);

            foreach (var match in matches)
            {
                await messageBus.Publish<IPostMatchFetchedMessage>(new PostMatchUpdatedResultMessage(match.Id, language.DisplayName, match.MatchResult));
            }
        }
    }
}