using System;
using System.Threading.Tasks;
using Fanex.Caching;

namespace Soccer.EventProcessors._Shared.Cache
{
    public interface ICacheManager
    {
        Task<T> GetOrFetch<T>(string cacheKey, Func<Task<T>> fetchFunc, CacheItemOptions cacheOptions);
    }
    public class CacheManager : ICacheManager
    {
        private readonly ICacheService cacheService;

        public CacheManager(ICacheService cacheService)
        {
            this.cacheService = cacheService;
        }

        public async Task<T> GetOrFetch<T>(string cacheKey, Func<Task<T>> fetchFunc, CacheItemOptions cacheOptions)
        {
            var cacheItems = await cacheService.GetAsync<T>(cacheKey);

            if (cacheItems == default)
            {
                cacheItems = await fetchFunc.Invoke();

                if (cacheItems != default)
                {
                    await cacheService.SetAsync(cacheKey, cacheItems, cacheOptions);
                }
            }

            return cacheItems;
        }
    }
}