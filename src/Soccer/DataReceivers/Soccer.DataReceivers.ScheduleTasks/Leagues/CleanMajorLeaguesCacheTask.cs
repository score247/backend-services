using System;
using System.Threading.Tasks;
using Hangfire;
using MassTransit;
using Soccer.Cache.Leagues;
using Soccer.Core.Leagues.QueueMessages;
using Soccer.DataProviders._Shared.Enumerations;
using Soccer.DataProviders.Leagues;

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
        private readonly ILeagueService leagueService;

        public CleanMajorLeaguesCacheTask(IBus messageBus, ILeagueCache leagueCache, Func<DataProviderType, ILeagueService> leagueServiceFactory)
        {
            this.messageBus = messageBus;
            this.leagueCache = leagueCache;
            this.leagueService = leagueServiceFactory(DataProviderType.Internal);
        }

        public async Task CleanMajorLeaguesCache()
        {
            await messageBus.Publish<IMajorLeaguesCacheCleanedMessage>(new MajorLeaguesCacheCleanedMessage());
            await leagueCache.ClearMajorLeaguesCache();
            await leagueService.ClearLeagueCache();
        }
    }
}