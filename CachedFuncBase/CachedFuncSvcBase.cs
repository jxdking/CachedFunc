using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MagicEastern.CachedFunc.Base
{
    public class CachedFuncSvcBase
    {
        private static int _funcID = 0;

        protected virtual ICacheHolder<TKey, TValue> GetCacheHolder<TKey, TValue>(CachedFuncOptions options)
        {
            if (options != null) {
                throw new NotSupportedException("CachedFuncSvcBase does not support any CachedFuncOptions!");
            }
            return new DictionaryCacheHolder<TKey, TValue>();
        }

        #region without cache policy, use Dictionary as cache
        public CachedFunc<TResult> Create<TResult>(
            Func<TResult> func = null,
            CachedFuncOptions options = null) 
        {
            CachedFunc<string, string, TResult> cf = CreateFunc<string, string, TResult>(Interlocked.Increment(ref _funcID), (i) => func(), PassThrough, options);
            CachedFunc<TResult> ret = (fallback, nocache) => cf("", (i) => fallback(), nocache);
            return ret;
        }

        public CachedFunc<T, TResult> Create<T, TResult>(
            Func<T, TResult> func = null, 
            CachedFuncOptions options = null)
        {
            CachedFunc<T, T, TResult> cf = CreateFunc<T, T, TResult>(Interlocked.Increment(ref _funcID), func, PassThrough, options);
            CachedFunc<T, TResult> ret = (input, fallback, nocache) => cf(input, fallback, nocache);
            return ret;
        }

        public CachedFunc<T, TKey, TResult> Create<T, TKey, TResult>(
            Func<T, TResult> func,
            Func<T, TKey> keySelector, 
            CachedFuncOptions options = null)
        {
            if (keySelector == null) { throw new ArgumentNullException("keySelector"); }
            CachedFunc<T, TKey, TResult> ret = CreateFunc(Interlocked.Increment(ref _funcID), func, keySelector, options);
            return ret;
        }

        private CachedFunc<T, TKey, TResult> CreateFunc<T, TKey, TResult>(
            int funcID,
            Func<T, TResult> func,
            Func<T, TKey> keySelector,
            CachedFuncOptions options)
        {
            ICacheHolder<TKey, TResult> cache = GetCacheHolder<TKey, TResult>(options);
            ConcurrentDictionary<TKey, object> locks = new ConcurrentDictionary<TKey, object>();
            CachedFunc<T, TKey, TResult> ret = (input, fallback, nocache) =>
            {
                TResult obj;
                TKey key = keySelector(input);
                if (!key.Equals(null))
                {
                    if (!nocache)
                    {
                        if (cache.TryGetValue(key, funcID, out obj))
                        {
                            return obj;
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
                    if (cache.TryGetValue(key, funcID, out obj))
                    {
                        return obj;
                    }
                    var fun = fallback ?? func;
                    if (fun != null)
                    {
                        obj = fun(input);
                        cache.Add(key, funcID, obj);
                        return obj;
                    }
                    throw new ArgumentNullException("Please provide a [fallback] function for calculating the value. ");
                } finally {
                    locks.TryRemove(key, out object o);
                    Monitor.Exit(lockObj);
                }
            };
            return ret;
        }
        #endregion

        protected T PassThrough<T>(T input) => input;
    }
}
