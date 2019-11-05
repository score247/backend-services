using System.Threading.Tasks;
using Hangfire;
using MassTransit;
using Soccer.Core.Leagues.QueueMessages;

namespace Soccer.DataReceivers.ScheduleTasks.Leagues
{
    public interface IFetchCleanMajorLeaguesCacheTask
    {
        [Queue("medium")]
        Task FetchCleanMajorLeaguesCache();
    }

    public class FetchCleanMajorLeaguesCacheTask : IFetchCleanMajorLeaguesCacheTask
    {
        private readonly IBus messageBus;

        public FetchCleanMajorLeaguesCacheTask(IBus messageBus)
        {
            this.messageBus = messageBus;
        }

        public async Task FetchCleanMajorLeaguesCache()
        {
            await messageBus.Publish<IMajorLeaguesCacheCleanedMessage>(new MajorLeaguesCacheCleanedMessage());
        }
    }
}