using MagicEastern.CachedFunc.Base;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace MagicEastern.CachedFunc.Core
{

    public class CachedFuncSvc : CachedFuncSvcBase
    {   
        private static IMemoryCache _defaultCache = new ServiceCollection().AddMemoryCache().BuildServiceProvider().GetService<IMemoryCache>();

        private IMemoryCache _cache;

        /// <summary>
        /// Create CachedFuncSvc object with optional IMemoryCache object.
        /// </summary>
        /// <param name="cache">If null, it will use default cache object from MemoryCache service.</param>
        public CachedFuncSvc(IMemoryCache cache = null)
        {
            _cache = cache ?? _defaultCache;
        }

        protected override ICacheHolder<TKey, TValue> GetCacheHolder<TKey, TValue>(CachedFuncOptions options)
        {
            if (options != null) {
                return new MemoryCacheHolder<TKey, TValue>(_cache, options);
            }
            return base.GetCacheHolder<TKey, TValue>(null);
        }
    }
}
