using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MagicEastern.CachedFunc
{
    /// <summary>
    /// Base class of CachedFuncSvc. Use this class directly is not recommended.
    /// </summary>
    public class CachedFuncSvcBase
    {
        private static int _funcID = 0;

        protected virtual ICacheHolder<TKey, TValue> GetCacheHolder<TKey, TValue>(CachedFuncOptions options)
        {
            if (options != null) {
                throw new NotSupportedException("CachedFuncSvcBase does not support any CachedFuncOptions!");
            }
            //without cache policy, use Dictionary as cache
            return new DictionaryCacheHolder<TKey, TValue>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult">Type of return object of func</typeparam>
        /// <param name="func">Pass null if you don't want define func at this moment.</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public CachedFunc<TResult> Create<TResult>(
            Func<TResult> func = null,
            CachedFuncOptions options = null) 
        {
            CachedFunc<string, string, TResult> cf = CreateFunc<string, string, TResult>(Interlocked.Increment(ref _funcID), (i) => func(), PassThrough, options);
            CachedFunc<TResult> ret = (fallback, nocache) => cf("", fallback == null ? null : new Func<string, TResult>((i) => fallback()), nocache);
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Type of input object of func</typeparam>
        /// <typeparam name="TResult">Type of return object of func</typeparam>
        /// <param name="func">Pass null if you don't want define func at this moment.</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public CachedFunc<T, TResult> Create<T, TResult>(
            Func<T, TResult> func = null, 
            CachedFuncOptions options = null)
        {
            CachedFunc<T, T, TResult> cf = CreateFunc<T, T, TResult>(Interlocked.Increment(ref _funcID), func, PassThrough, options);
            CachedFunc<T, TResult> ret = (input, fallback, nocache) => cf(input, fallback, nocache);
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Type of input object of func</typeparam>
        /// <typeparam name="TKey">Type of return object of keySelector</typeparam>
        /// <typeparam name="TResult">Type of return object of func</typeparam>
        /// <param name="func">Pass null if you don't want define func at this moment.</param>
        /// <param name="keySelector">The result of keySelector will be use as key for internal Dictonary or MemoryCache object.</param>
        /// <param name="options"></param>
        /// <returns></returns>
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

                object lockObj = locks.GetOrAdd(key, new object());
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

        protected T PassThrough<T>(T input) => input;
    }
}
