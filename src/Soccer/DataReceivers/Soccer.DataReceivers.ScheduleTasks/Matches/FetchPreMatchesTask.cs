namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit;
    using Score247.Shared.Enumerations;
    using Soccer.Core._Shared.Enumerations;
    using Soccer.Core.Matches.Events;
    using Soccer.DataProviders.Matches.Services;
    using Soccer.DataReceivers.ScheduleTasks._Shared.Configurations;

    public interface IFetchPreMatchesTask
    {
        Task FetchPreMatches(int dateSpan);
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

        public async Task FetchPreMatches(int dateSpan)
        {
            var from = DateTime.UtcNow;
            var to = DateTime.UtcNow.AddDays(dateSpan);

            foreach (var language in Enumeration.GetAll<Language>())
            {
                await FetchPreMatches(from, to, language);
            }
        }

        private async Task FetchPreMatches(DateTime from, DateTime to, Language language)
        {
            int batchSize = appSettings.ScheduleTasksSettings.QueueBatchSize;
            var matches = await matchService.GetPreMatches(from, to, language);

            for (var i = 0; i * batchSize < matches.Count; i++)
            {
                var matchesBatch = matches.Skip(i * batchSize).Take(batchSize);

                await messageBus.Publish<IPreMatchesFetchedEvent>(new { Matches = matchesBatch, Language = language.DisplayName });
            }
        }
    }
}