using StackExchange.Redis;
using System.Threading.RateLimiting;

namespace WebAPI.Middleware
{
    /// <summary>
    /// Redis-based distributed rate limit storage for multi-instance deployments
    /// </summary>
    public class RedisRateLimitStorage
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly string _instanceName;

        public RedisRateLimitStorage(IConnectionMultiplexer redis, string instanceName)
        {
            _redis = redis;
            _instanceName = instanceName;
        }

        public async Task<bool> TryAcquireAsync(string key, int permitLimit, TimeSpan window)
        {
            var db = _redis.GetDatabase();
            var redisKey = $"{_instanceName}RateLimit:{key}";
            
            var count = await db.StringIncrementAsync(redisKey);
            
            if (count == 1)
            {
                // First request, set expiration
                await db.KeyExpireAsync(redisKey, window);
            }
            
            return count <= permitLimit;
        }

        public async Task<long> GetCurrentCountAsync(string key)
        {
            var db = _redis.GetDatabase();
            var redisKey = $"{_instanceName}RateLimit:{key}";
            var value = await db.StringGetAsync(redisKey);
            
            return value.HasValue ? (long)value : 0;
        }

        public async Task ResetAsync(string key)
        {
            var db = _redis.GetDatabase();
            var redisKey = $"{_instanceName}RateLimit:{key}";
            await db.KeyDeleteAsync(redisKey);
        }
    }
}
