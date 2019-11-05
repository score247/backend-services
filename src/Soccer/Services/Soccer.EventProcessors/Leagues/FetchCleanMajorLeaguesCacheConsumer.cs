using System.Threading.Tasks;
using MassTransit;
using Score247.Shared;
using Soccer.Core.Leagues.QueueMessages;

namespace Soccer.EventProcessors.Leagues
{
    public class FetchCleanMajorLeaguesCacheConsumer : IConsumer<IMajorLeaguesCacheCleanedMessage>
    {
        private const string MajorLeaguesCacheKey = "Major_Leagues";
        private readonly ICacheManager cacheManager;

        public FetchCleanMajorLeaguesCacheConsumer(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
        }

        public async Task Consume(ConsumeContext<IMajorLeaguesCacheCleanedMessage> context)
        {
            await cacheManager.RemoveAsync(MajorLeaguesCacheKey);
        }
    }
}