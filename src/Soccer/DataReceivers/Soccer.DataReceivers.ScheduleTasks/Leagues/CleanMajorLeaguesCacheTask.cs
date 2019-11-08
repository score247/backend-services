using System.Threading.Tasks;
using Hangfire;
using MassTransit;
using Soccer.Cache.Leagues;
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
        private readonly ILeagueCache leagueCache;

        public CleanMajorLeaguesCacheTask(IBus messageBus, ILeagueCache leagueCache)
        {
            this.messageBus = messageBus;
            this.leagueCache = leagueCache;
        }

        public async Task CleanMajorLeaguesCache()
        {
            await messageBus.Publish<IMajorLeaguesCacheCleanedMessage>(new MajorLeaguesCacheCleanedMessage());
            await leagueCache.ClearMajorLeaguesCache();
        }
    }
}