using Microsoft.Extensions.Caching.Memory;
using MagicEastern.CachedFunc.Base;

namespace MagicEastern.CachedFunc.Core
{
    class MemoryCacheHolder<TKey, TValue> : ICacheHolder<TKey, TValue>
    {
        private IMemoryCache _cache;
        private MemoryCacheEntryOptions _options;

        public MemoryCacheHolder(IMemoryCache cache, CachedFuncOptions options)
        {
            _cache = cache;
            _options = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = options.AbsoluteExpiration,
                AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow,
                SlidingExpiration = options.SlidingExpiration
            };
        }

        public bool TryGetValue(TKey key, int funcID, out TValue val) {
            return _cache.TryGetValue<TValue>(new CacheKey<TKey>(funcID, key), out val);
        }

        public void Add(TKey key, int funcID, TValue val) {
            _cache.Set<TValue>(new CacheKey<TKey>(funcID, key), val, _options);
        }
    }
}
