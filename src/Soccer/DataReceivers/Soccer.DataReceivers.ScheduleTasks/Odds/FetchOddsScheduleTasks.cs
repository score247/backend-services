using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using MassTransit;
using Soccer.Core.Odds.Messages;
using Soccer.Core.Odds.Models;
using Soccer.DataProviders.Odds;
using Soccer.DataReceivers.ScheduleTasks.Shared.Configurations;

namespace Soccer.DataReceivers.ScheduleTasks.Odds
{
    public interface IFetchOddsScheduleTask
    {
        [Queue("lowlive")]
        Task FetchOdds();

        [Queue("mediumlive")]
        Task FetchOddsChangeLogs();
    }

    public class FetchOddsScheduleTask : IFetchOddsScheduleTask
    {
        private readonly ScheduleTasksSettings scheduleTaskSettings;
        private readonly IOddsService oddsService;
        private readonly IBus messageBus;

        public FetchOddsScheduleTask(
            IAppSettings appSettings,
            IOddsService oddsService,
            IBus messageBus)
        {
            this.scheduleTaskSettings = appSettings.ScheduleTasksSettings;
            this.oddsService = oddsService;
            this.messageBus = messageBus;
        }

        public async Task FetchOddsChangeLogs()
        {
            var oddsList = await oddsService.GetOddsChange(scheduleTaskSettings.FetchOddsChangeMinuteInterval);

            await PublishOdds(oddsList);
        }

        public async Task FetchOdds()
        {
            var oddsList = await oddsService.GetOdds();

            await PublishOdds(oddsList);
        }

        private async Task PublishOdds(IEnumerable<MatchOdds> oddsList)
        {
            var total = oddsList.Count();
            var batchSize = scheduleTaskSettings.QueueBatchSize;

            for (var batchIndex = 0; batchIndex * batchSize < total; batchIndex++)
            {
                var oddsBatch = oddsList.Skip(batchIndex * batchSize).Take(batchSize);

                await messageBus.Publish<IOddsChangeMessage>(new OddsChangeMessage(oddsBatch));
            }
        }
    }
}