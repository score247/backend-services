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

    public interface IFetchPostMatchesTask
    {
        Task FetchPostMatches(int dateSpan);
    }

    public class FetchPostMatchesTask : IFetchPostMatchesTask
    {
        private readonly IMatchService matchService;
        private readonly IBus messageBus;
        private readonly IAppSettings appSettings;

        public FetchPostMatchesTask(IBus messageBus, IAppSettings appSettings, IMatchService matchService)
        {
            this.appSettings = appSettings;
            this.messageBus = messageBus;
            this.matchService = matchService;
        }

        public async Task FetchPostMatches(int dateSpan)
        {
            var from = DateTime.UtcNow.AddDays(-dateSpan);
            var to = DateTime.UtcNow;

            foreach (var language in Enumeration.GetAll<Language>())
            {
                await FetchPostMatches(from, to, language);
            }
        }

        private async Task FetchPostMatches(DateTime from, DateTime to, Language language)
        {
            int batchSize = appSettings.ScheduleTasksSettings.QueueBatchSize;
            var matches = await matchService.GetPostMatches(from, to, language);

            for (var i = 0; i * batchSize < matches.Count; i++)
            {
                var updatedMatches = matches.Skip(i * batchSize).Take(batchSize);

                await messageBus.Publish<IPostMatchUpdatedEvent>(new { Matches = updatedMatches, Language = language.DisplayName });
            }
        }
    }
}