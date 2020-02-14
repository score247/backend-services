using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Caching;
using Score247.Shared;
using Score247.Shared.Constants;

namespace Soccer.EventProcessors.Notifications
{
    public interface IMatchNotificationDeduper
    {
        Task<bool> IsProcessedAsync(string id, string content);
    }

    public class MatchNotificationDeduper : IMatchNotificationDeduper
    {
        private static readonly CacheItemOptions NotificationCacheOptions = new CacheItemOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(360), //Max minutes of a match
        };

        private readonly ICacheManager cacheManager;

        public MatchNotificationDeduper(ICacheManager cacheManager)
        => this.cacheManager = cacheManager;

        public async Task<bool> IsProcessedAsync(string id, string content)
        {
            var matchNotificationCacheKey = $"{CacheKeys.MATCH_NOTIFICATION_CACHE_KEY}_{id}";
            var processedNotifications = await GetProcessedNotificationsAsync(matchNotificationCacheKey);

            var isProcessed = processedNotifications.Contains(content);

            if (!isProcessed)
            {
                processedNotifications.Add(content);
            }

            return isProcessed;
        }

        private async Task<BlockingCollection<string>> GetProcessedNotificationsAsync(string matchNotificationCacheKey)
        => await cacheManager.GetOrSetAsync(
                            matchNotificationCacheKey,
                            async () =>
                            {
                                return await Task.FromResult(new BlockingCollection<string>());
                            },
                            NotificationCacheOptions);
    }
}
