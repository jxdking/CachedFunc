using MagicEastern.CachedFuncBase;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

namespace MagicEastern.CachedFuncCore
{

    public class CachedFuncSvc : CachedFuncSvcBase
    {
        #region with cache policy, use MemoryCache as cache
        private static IMemoryCache _defaultCache = new ServiceCollection().AddMemoryCache().BuildServiceProvider().GetService<IMemoryCache>();

        private IMemoryCache _cache;
        public CachedFuncSvc(IMemoryCache cache = null)
        {
            _cache = cache ?? _defaultCache;
        }

        private static int _funcID = 0;

        public CachedFunc<TResult> Create<TResult>(
            Func<TResult> func,
            MemoryCacheEntryOptions cachePolicy)
        {
            CachedFunc<string, TResult> cachedFunc = Create(AddInputToFunc<string, TResult>(func), cachePolicy);
            CachedFunc<TResult> ret = (fallback, nocache) => cachedFunc("", AddInputToFunc<string, TResult>(fallback), nocache);
            return ret;
        }

        public CachedFunc<T, TResult> Create<T, TResult>(
            Func<T, TResult> func,
            MemoryCacheEntryOptions cachePolicy)
        {
            var cache = _cache;
            CachedFunc<T, T, TResult> cf = CreateFunc(cache, Interlocked.Increment(ref _funcID), func, cachePolicy, PassThrough);
            CachedFunc<T, TResult> ret = (input, fallback, nocache) => cf(input, fallback, nocache);
            return ret;
        }

        public CachedFunc<T, TKey, TResult> Create<T, TKey, TResult>(
            Func<T, TResult> func,
            MemoryCacheEntryOptions cachePolicy,
            Func<T, TKey> keySelector)
        {
            if (keySelector == null) { throw new ArgumentNullException("keySelector"); }
            var cache = _cache;
            CachedFunc<T, TKey, TResult> ret = CreateFunc(cache, Interlocked.Increment(ref _funcID), func, cachePolicy, keySelector);
            return ret;
        }

        private CachedFunc<T, TKey, TResult> CreateFunc<T, TKey, TResult>(
            IMemoryCache cache,
            int funcID,
            Func<T, TResult> func,
            MemoryCacheEntryOptions cachePolicy,
            Func<T, TKey> keySelector)
        {
            CachedFunc<T, TKey, TResult> ret = (key, fallback, nocache) =>
            {
                CacheKey<TKey> cacheKey = new CacheKey<TKey> { FuncID = funcID, Value = keySelector(key) };
                if (!nocache)
                {
                    if (cache.TryGetValue(cacheKey, out TResult obj))
                    {
                        return obj;
                    }
                }
                var fun = fallback ?? func;
                if (fun != null)
                {
                    TResult res = fun(key);
                    cache.Set(cacheKey, res, cachePolicy);
                    return res;
                }
                throw new ArgumentNullException("Please provide a [fallback] function for calculating the value. ");
            };
            return ret;
        }
        #endregion

        private Func<T, TResult> AddInputToFunc<T, TResult>(Func<TResult> func)
        {
            if (func == null)
            {
                return null;
            }
            return (input) => func();
        }
    }
}
