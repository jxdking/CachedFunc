using System.Runtime.Caching;

namespace MagicEastern.CachedFunc.Net45
{
    public class CachedFuncSvc : CachedFuncSvcBase
    {
        protected override ICacheHolder<TKey, TValue> GetCacheHolder<TKey, TValue>(CachedFuncOptions options)
        {
            if (options != null) {
                return new MemoryCacheHolder<TKey, TValue>(MemoryCache.Default, options); 
            }
            return base.GetCacheHolder<TKey, TValue>(null);
        }
    }
}
