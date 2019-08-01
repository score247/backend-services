namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Hangfire;
    using MassTransit;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Events;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.DataProviders.Matches.Services;
    using Soccer.DataReceivers.ScheduleTasks.Shared.Configurations;

    public interface IFetchPreMatchesTask
    {
        void FetchPreMatches(int dateSpan);

        Task FetchPreMatchesForDate(DateTime date, Language language);
    }

    public class FetchPreMatchesTask : IFetchPreMatchesTask
    {
        private readonly IAppSettings appSettings;
        private readonly IMatchService matchService;
        private readonly IBus messageBus;

        public FetchPreMatchesTask(IBus messageBus, IAppSettings appSettings, IMatchService matchService)
        {
            this.appSettings = appSettings;
            this.messageBus = messageBus;
            this.matchService = matchService;
        }

        public void FetchPreMatches(int dateSpan)
        {
            var from = DateTime.UtcNow;
            var to = DateTime.UtcNow.AddDays(dateSpan);

            foreach (var language in Enumeration.GetAll<Language>())
            {
                for (var date = from; date.Date <= to; date = date.AddDays(1))
                {
                    BackgroundJob.Enqueue(() => FetchPreMatchesForDate(date, language));
                }
            }
        }

        public async Task FetchPreMatchesForDate(DateTime date, Language language)
        {
            int batchSize = appSettings.ScheduleTasksSettings.QueueBatchSize;
            var matches = await matchService.GetPreMatches(date, language);

            for (var i = 0; i * batchSize < matches.Count; i++)
            {
                var matchesBatch = matches.Skip(i * batchSize).Take(batchSize);

                await messageBus.Publish<IPreMatchesFetchedMessage>(new PreMatchesFetchedMessage(matchesBatch, language.DisplayName));
            }
        }
    }
}