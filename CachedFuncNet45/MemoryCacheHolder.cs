using MagicEastern.CachedFunc.Base;
using System;
using System.Collections.Concurrent;
using System.Runtime.Caching;
using System.Threading;


namespace MagicEastern.CachedFunc.Net45
{
    class MemoryCacheHolder<TKey, TValue> : ICacheHolder<TKey, TValue>
    {
        private ObjectCache _cache;
        private Func<CacheItemPolicy> _optionsFactory;
        private int objID = 0;
        private ConcurrentDictionary<CacheKey<TKey>, string> objIDDic = new ConcurrentDictionary<CacheKey<TKey>, string>();

        public MemoryCacheHolder(ObjectCache cache, CachedFuncOptions options)
        {
            _cache = cache;

            if (options.AbsoluteExpirationRelativeToNow != null)
            {
                if (options.AbsoluteExpiration != null)
                {
                    _optionsFactory = () =>
                    {
                        DateTimeOffset abs = DateTimeOffset.Now.Add((TimeSpan)options.AbsoluteExpirationRelativeToNow);
                        if (abs > options.AbsoluteExpiration)
                        {
                            abs = (DateTimeOffset)options.AbsoluteExpiration;
                        }
                        var ret = new CacheItemPolicy
                        {
                            AbsoluteExpiration = abs,
                            SlidingExpiration = options.SlidingExpiration ?? default(TimeSpan)
                        };
                        return ret;
                    };
                    return;
                }
                _optionsFactory = () => new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTimeOffset.Now.Add((TimeSpan)options.AbsoluteExpirationRelativeToNow),
                    SlidingExpiration = options.SlidingExpiration ?? default(TimeSpan)
                };
                return;
            }
            CacheItemPolicy po = new CacheItemPolicy
            {
                AbsoluteExpiration = options.AbsoluteExpiration ?? DateTimeOffset.MaxValue,
                SlidingExpiration = options.SlidingExpiration ?? default(TimeSpan)
            };
            _optionsFactory = () => po;
        }

        public bool TryGetValue(TKey key, int funcID, out TValue val)
        {
            if (!TryGetCacheKey(key, funcID, out string cacheKey, false)) {
                val = default(TValue);
                return false;
            }
            object obj = _cache[cacheKey];
            if (obj != null)
            {
                val = (TValue)obj;
                return true;
            }
            val = default(TValue);
            return false;
        }

        public void Add(TKey key, int funcID, TValue val)
        {
            TryGetCacheKey(key, funcID, out string cacheKey, true);
            _cache.Set(cacheKey, val, _optionsFactory());
            return;
        }

        private bool TryGetCacheKey(TKey key, int funcID, out string cacheKey, bool addIfFailed) {
            CacheKey<TKey> ck = new CacheKey<TKey>(funcID, key);
            if (objIDDic.TryGetValue(ck, out cacheKey)) {
                return true;
            }
            cacheKey = "CachedFunc" + funcID + "_Obj" + Interlocked.Increment(ref objID).ToString();
            if (addIfFailed) {
                objIDDic.TryAdd(ck, cacheKey);    
            }
            return false;
        }
    }
}
