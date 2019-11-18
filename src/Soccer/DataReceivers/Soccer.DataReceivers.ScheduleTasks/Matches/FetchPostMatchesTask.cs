using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using MassTransit;
using Score247.Shared.Enumerations;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Events;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders._Shared.Enumerations;
using Soccer.DataProviders.Leagues;
using Soccer.DataProviders.Matches.Services;
using Soccer.DataReceivers.ScheduleTasks.Shared.Configurations;

namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    public interface IFetchPostMatchesTask
    {
        [AutomaticRetry(Attempts = 1)]
        [Queue("low")]
        Task FetchPostMatches(int dateSpan);

        [AutomaticRetry(Attempts = 1)]
        [Queue("low")]
        Task FetchPostMatchesForDate(DateTime date, Language language, IEnumerable<League> majorLeagues);
    }

    public class FetchPostMatchesTask : IFetchPostMatchesTask
    {
        private readonly IMatchService matchService;
        private readonly IBus messageBus;
        private readonly IAppSettings appSettings;
        private readonly ILeagueService internalLeagueService;

        public FetchPostMatchesTask(
            IBus messageBus,
            IAppSettings appSettings,
            IMatchService matchService,
            Func<DataProviderType, ILeagueService> leagueServiceFactory)
        {
            this.appSettings = appSettings;
            this.messageBus = messageBus;
            this.matchService = matchService;
            internalLeagueService = leagueServiceFactory(DataProviderType.Internal);
        }

        public async Task FetchPostMatches(int dateSpan)
        {
            var majorLeagues = await internalLeagueService.GetLeagues(Language.en_US);

            foreach (var language in Enumeration.GetAll<Language>())
            {
                for (var dayAdd = 0; dayAdd <= dateSpan; dayAdd++)
                {
                    var fetchDate = DateTime.UtcNow.AddDays(-dayAdd);

                    if (dayAdd > 0)
                    {
                        BackgroundJob.Schedule<IFetchPostMatchesTask>(
                            t => t.FetchPostMatchesForDate(fetchDate, language, majorLeagues),
                            TimeSpan.FromHours(appSettings.ScheduleTasksSettings.FetchMatchesByDateDelayedHours * dayAdd));
                    }
                    else
                    {
                        BackgroundJob.Enqueue<IFetchPostMatchesTask>(
                            t => t.FetchPostMatchesForDate(fetchDate, language, majorLeagues));
                    }
                }
            }
        }

        public async Task FetchPostMatchesForDate(DateTime date, Language language, IEnumerable<League> majorLeagues)
        {
            var batchSize = appSettings.ScheduleTasksSettings.QueueBatchSize;
            var matches = (await matchService.GetPostMatches(date, language))
                .Where(match => majorLeagues?.Any(league => league.Id == match.League.Id) == true)
                .ToList();

            for (var i = 0; i * batchSize < matches.Count; i++)
            {
                var matchesBatch = matches.Skip(i * batchSize).Take(batchSize);

                await messageBus.Publish<IPostMatchFetchedMessage>(new PostMatchFetchedMessage(matchesBatch, language.DisplayName));
            }
        }
    }
}