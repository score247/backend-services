namespace Score247.Shared
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Caching;

    public interface ICacheManager
    {
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, CacheItemOptions options);

        Task<T> GetAsync<T>(string key);

        Task SetAsync<T>(string key, T value, CacheItemOptions options);

        Task RemoveAsync(string key);
    }

    public class CacheManager : ICacheManager
    {
        private readonly ICacheService cacheService;

        public CacheManager(ICacheService cacheService)
        {
            this.cacheService = cacheService;
        }

        public Task<T> GetAsync<T>(string key)
            => cacheService.GetAsync<T>(key);

        public Task SetAsync<T>(string key, T value, CacheItemOptions options)
            => cacheService.SetAsync(key, value, options);

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, CacheItemOptions options)
        {
            var (found, value) = await cacheService.TryGetAsync<T>(key).ConfigureAwait(false);

            if (!found && factory != null)
            {
                value = await factory();

                await cacheService.SetAsync(key, value, options).ConfigureAwait(false);
            }

            return value;
        }

        public Task RemoveAsync(string key)
            => cacheService.RemoveAsync(key);
    }
}