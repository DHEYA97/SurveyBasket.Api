using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace SurveyBasket.Api.Services
{
    public class CacheService(IDistributedCache distributedCache) : ICacheService
    {
        private readonly IDistributedCache _distributedCache = distributedCache;
        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            var value = await _distributedCache.GetStringAsync(key, cancellationToken);
            return string.IsNullOrEmpty(value) ? null : JsonSerializer.Deserialize<T>(value);

        }
        public async Task SetAsync<T>(string key, T Value, CancellationToken cancellationToken) where T : class
        {
            var value = await GetAsync<T>(key, cancellationToken);
            if(value is null)
                await _distributedCache.SetStringAsync(key,JsonSerializer.Serialize(value),cancellationToken);
        }
        public async Task RemoveAsync(string key, CancellationToken cancellationToken)
        {
           await _distributedCache.RemoveAsync(key, cancellationToken);
        }
    }
}
