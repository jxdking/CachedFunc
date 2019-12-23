using Microsoft.Extensions.Caching.Memory;

namespace MagicEastern.CachedFunc.Core
{

    public class CachedFuncSvc : CachedFuncSvcBase
    {
        public static CachedFuncSvc Default;

        static CachedFuncSvc()
        {
            Default = new CachedFuncSvc();
        }

        private static IMemoryCache _defaultCache = new MemoryCache(new MemoryCacheOptions());

        private IMemoryCache _cache;

        /// <summary>
        /// Create CachedFuncSvc object with optional IMemoryCache object.
        /// </summary>
        /// <param name="cache">If null, it will create a default cache object from MemoryCache.</param>
        public CachedFuncSvc(IMemoryCache cache = null)
        {
            _cache = cache ?? _defaultCache;
        }

        protected override ICacheHolder<TKey, TValue> GetCacheHolder<TKey, TValue>(CachedFuncOptions options)
        {
            if (options != null)
            {
                return new MemoryCacheHolder<TKey, TValue>(_cache, options);
            }
            return base.GetCacheHolder<TKey, TValue>(null);
        }
    }
}
