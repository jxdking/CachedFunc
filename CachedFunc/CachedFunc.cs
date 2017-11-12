using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MagicEastern.CachedFunc
{
    public delegate TResult CachedFunc<TResult>(Func<TResult> fallback = null, bool nocache = false);
    public delegate TResult CachedFunc<T, TResult>(T input, Func<T, TResult> fallback = null, bool nocache = false);
    public delegate TResult CachedFunc<T, TKey, TResult>(T input, Func<T, TResult> fallback = null, bool nocache = false);

    public class CachedFuncSvc
    {
        #region without cache policy, use Dictionary as cache
        public CachedFunc<T, TResult> Create<T, TResult>(Func<T, TResult> func = null)
        {
            var cache = new ConcurrentDictionary<T, TResult>();
            CachedFunc<T, T, TResult> cf = CreateFunc(cache, func, PassThrough);
            CachedFunc<T, TResult> ret = (input, fallback, nocache) => cf(input, fallback, nocache);
            return ret;
        }

        public CachedFunc<T, TKey, TResult> Create<T, TKey, TResult>(
            Func<T, TResult> func, 
            Func<T, TKey> keySelector)
        {
            if (keySelector == null) { throw new ArgumentNullException("keySelector"); }
            var cache = new ConcurrentDictionary<TKey, TResult>();
            CachedFunc<T, TKey, TResult> ret = CreateFunc(cache, func, keySelector);
            return ret;
        }

        private CachedFunc<T, TKey, TResult> CreateFunc<T, TKey, TResult>(
            ConcurrentDictionary<TKey, TResult> cache, 
            Func<T, TResult> func, 
            Func<T, TKey> keySelector)
        {
            CachedFunc<T, TKey, TResult> ret = (input, fallback, nocache) =>
            {
                var fun = fallback ?? func;
                if (fun != null)
                {
                    TResult obj;
                    TKey key = keySelector(input);
                    Func<TKey, TResult> addFunc = (k) => fun(input);
                    if (!nocache)
                    {
                        if (cache.TryGetValue(key, out obj)) {
                            return obj;
                        }
                    }
                    obj = fun(input);
                    cache.GetOrAdd(key, obj);
                    return obj;
                }
                throw new ArgumentNullException("Please provide a [fallback] function for calculating the value. ");
            };
            return ret;
        }
        #endregion


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

        private T PassThrough<T>(T input) => input;

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
