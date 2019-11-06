using System.Threading.Tasks;
using Hangfire;
using MassTransit;
using Soccer.Core.Leagues.QueueMessages;

namespace Soccer.DataReceivers.ScheduleTasks.Leagues
{
    public interface ICleanMajorLeaguesCacheTask
    {
        [Queue("low")]
        Task CleanMajorLeaguesCache();
    }

    public class CleanMajorLeaguesCacheTask : ICleanMajorLeaguesCacheTask
    {
        private readonly IBus messageBus;

        public CleanMajorLeaguesCacheTask(IBus messageBus)
        {
            this.messageBus = messageBus;
        }

        public async Task CleanMajorLeaguesCache()
        {
            await messageBus.Publish<IMajorLeaguesCacheCleanedMessage>(new MajorLeaguesCacheCleanedMessage());
        }
    }
}