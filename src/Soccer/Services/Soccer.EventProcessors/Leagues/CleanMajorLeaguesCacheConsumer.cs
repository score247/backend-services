using System.Threading.Tasks;
using MassTransit;
using Soccer.Cache.Leagues;
using Soccer.Core.Leagues.QueueMessages;

namespace Soccer.EventProcessors.Leagues
{
    public class CleanMajorLeaguesCacheConsumer : IConsumer<IMajorLeaguesCacheCleanedMessage>
    {
        private readonly ILeagueCache leagueCache;

        public CleanMajorLeaguesCacheConsumer(ILeagueCache leagueCache)
        {
            this.leagueCache = leagueCache;
        }

        public Task Consume(ConsumeContext<IMajorLeaguesCacheCleanedMessage> context)
            => leagueCache.ClearMajorLeaguesCache();
    }
}