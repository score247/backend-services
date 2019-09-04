namespace Soccer.API.Shared.Services
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Caching;

    public interface IDateCacheService
    {
        Task<T> GetOrSetAsync<T>(string key, DateTime from, DateTime to, Func<T> factory);
    }

    public class DateCacheService : IDateCacheService
    {
        private readonly ICacheService cacheService;
        private const string formatDate = "yyyyMMdd-hhmmss";

        public DateCacheService(ICacheService cacheService)
        {
            this.cacheService = cacheService;
        }

        public async Task<T> GetOrSetAsync<T>(string key, DateTime from, DateTime to, Func<T> factory)
        {
            var cacheDuration = ShouldCacheWithShortDuration(from)
                ? CacheDuration.Short
                : CacheDuration.Long;
            var cacheItemOptions = new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(0, 0, (int)cacheDuration));
            var cacheKey = BuildCacheKey(key, from, to);

            return await cacheService
                .GetOrSetAsync(cacheKey, factory, cacheItemOptions);
        }

        private static bool ShouldCacheWithShortDuration(DateTime date)
            => date.ToUniversalTime().Date == DateTime.UtcNow.Date
                || date.ToUniversalTime().Date == DateTime.UtcNow.Date.AddDays(-1);

        private static string BuildCacheKey(string key, DateTime from, DateTime to)
            => $"{key}_{from.ToString(formatDate)}_{to.ToString(formatDate)}";
    }

    public enum CacheDuration
    {
        Short = 30, // seconds
        Long = 3600
    }
}