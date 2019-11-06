using System.Threading.Tasks;
using MassTransit;
using Score247.Shared;
using Soccer.Core.Leagues.QueueMessages;

namespace Soccer.EventProcessors.Leagues
{
    public class CleanMajorLeaguesCacheConsumer : IConsumer<IMajorLeaguesCacheCleanedMessage>
    {
        private const string MajorLeaguesCacheKey = "Major_Leagues";
        private readonly ICacheManager cacheManager;

        public CleanMajorLeaguesCacheConsumer(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
        }

        public async Task Consume(ConsumeContext<IMajorLeaguesCacheCleanedMessage> context)
        {
            await cacheManager.RemoveAsync(MajorLeaguesCacheKey);
        }
    }
}