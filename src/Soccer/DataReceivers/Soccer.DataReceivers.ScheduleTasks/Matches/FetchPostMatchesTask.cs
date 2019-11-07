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

namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    public interface IFetchPostMatchesTask
    {
        [Queue("low")]
        void FetchPostMatches(int dateSpan);

        [Queue("low")]
        Task FetchPostMatchesForDate(DateTime date, Language language);
    }

    public class FetchPostMatchesTask : IFetchPostMatchesTask
    {
        private readonly IMatchService matchService;
        private readonly IBus messageBus;
        private readonly IAppSettings appSettings;

        public FetchPostMatchesTask(
            IBus messageBus,
            IAppSettings appSettings,
            IMatchService matchService)
        {
            this.appSettings = appSettings;
            this.messageBus = messageBus;
            this.matchService = matchService;
        }

        public void FetchPostMatches(int dateSpan)
        {
            foreach (var language in Enumeration.GetAll<Language>())
            {
                for (var dayAdd = 0; dayAdd <= dateSpan; dayAdd++)
                {
                    var fetchDate = DateTime.UtcNow.AddDays(-dayAdd);

                    if (dayAdd > 0)
                    {
                        BackgroundJob.Schedule<IFetchPostMatchesTask>(
                            t => t.FetchPostMatchesForDate(fetchDate, language),
                            TimeSpan.FromHours(appSettings.ScheduleTasksSettings.FetchMatchesByDateDelayedHours * dayAdd));
                    }
                    else
                    {
                        BackgroundJob.Enqueue<IFetchPostMatchesTask>(
                            t => t.FetchPostMatchesForDate(fetchDate, language));
                    }
                }
            }
        }

        public async Task FetchPostMatchesForDate(DateTime date, Language language)
        {
            var batchSize = appSettings.ScheduleTasksSettings.QueueBatchSize;
            var matches = await matchService.GetPostMatches(date, language);

            for (var i = 0; i * batchSize < matches.Count; i++)
            {
                var matchesBatch = matches.Skip(i * batchSize).Take(batchSize);

                await messageBus.Publish<IPostMatchFetchedMessage>(new PostMatchFetchedMessage(matchesBatch, language.DisplayName));
            }
        }
    }
}