using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MagicEastern.CachedFuncBase
{
    public abstract class CachedFuncSvcBase
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
            ConcurrentDictionary<TKey, object> locks = new ConcurrentDictionary<TKey, object>();
            CachedFunc<T, TKey, TResult> ret = (input, fallback, nocache) =>
            {
                TResult obj;
                TKey key = keySelector(input);
                if (key != null)
                {
                    if (!nocache)
                    {
                        if (cache.TryGetValue(key, out obj))
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
                    if (cache.TryGetValue(key, out obj))
                    {
                        return obj;
                    }
                    var fun = fallback ?? func;
                    if (fun != null)
                    {
                        obj = fun(input);
                        cache.TryAdd(key, obj);
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
