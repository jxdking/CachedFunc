using MagicEastern.CachedFuncBase;
using System;
using System.Collections.Concurrent;
using System.Runtime.Caching;
using System.Threading;

namespace MagicEastern.CachedFunc
{
    public class CachedFuncSvc : CachedFuncSvcBase
    {
        #region with cache policy, use MemoryCache as cache
        private static int _funcID = 0;

        public CachedFunc<TResult> Create<TResult>(Func<TResult> func, Func<CacheItemPolicy> policyFactory)
        {
            CachedFunc<string, TResult> cachedFunc = Create(AddInputToFunc<string, TResult>(func), policyFactory);
            CachedFunc<TResult> ret = (fallback, nocache) => cachedFunc("", AddInputToFunc<string, TResult>(fallback), nocache);
            return ret;
        }

        public CachedFunc<string, TResult> Create<TResult>(Func<string, TResult> func, Func<CacheItemPolicy> policyFactory)
        {
            var cache = MemoryCache.Default;
            CachedFunc<string, TResult> ret = CreateFunc(cache, Interlocked.Increment(ref _funcID), func, policyFactory, PassThrough);
            return ret;
        }

        public CachedFunc<T, TResult> Create<T, TResult>(
            Func<T, TResult> func,
            Func<CacheItemPolicy> policyFactory,
            Func<T, string> keySelector)
        {
            if (keySelector == null) { throw new ArgumentNullException("keySelector"); }
            var cache = MemoryCache.Default;
            CachedFunc<T, TResult> ret = CreateFunc(cache, Interlocked.Increment(ref _funcID), func, policyFactory, keySelector);
            return ret;
        }

        private CachedFunc<T, TResult> CreateFunc<T, TResult>(
            MemoryCache cache,
            int funcID,
            Func<T, TResult> func,
            Func<CacheItemPolicy> policyFactory,
            Func<T, string> keySelector)
        {
            ConcurrentDictionary<string, object> locks = new ConcurrentDictionary<string, object>();
            CachedFunc<T, TResult> ret = (input, fallback, nocache) =>
            {
                string key = keySelector(input);
                string cacheKey = "CachedFunc" + _funcID.ToString() + keySelector(input);
                if (key != null)
                {
                    if (!nocache)
                    {
                        object obj = cache[cacheKey];
                        if (obj != null)
                        {
                            return (TResult)obj;
                        }
                    }
                }
                else
                {
                    throw new ArgumentNullException("[input] of the function is null or [keySelector(T)] returns null.");
                }

                object lockObj = new object();
                lockObj = locks.GetOrAdd(key, lockObj);
                Monitor.Enter(lockObj);
                try
                {
                    object obj = cache[cacheKey];
                    if (obj != null)
                    {
                        return (TResult)obj;
                    }

                    var fun = fallback ?? func;
                    if (fun != null)
                    {
                        TResult res = fun(input);
                        cache.Set(cacheKey, res, policyFactory());
                        return res;
                    }
                    throw new ArgumentNullException("Please provide a [fallback] function for calculating the value. ");
                }
                finally
                {
                    locks.TryRemove(key, out object o);
                    Monitor.Exit(lockObj);
                }
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
