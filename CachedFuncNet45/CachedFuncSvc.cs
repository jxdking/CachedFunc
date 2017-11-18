using MagicEastern.CachedFunc.Base;
using System;
using System.Collections.Concurrent;
using System.Runtime.Caching;
using System.Threading;

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
