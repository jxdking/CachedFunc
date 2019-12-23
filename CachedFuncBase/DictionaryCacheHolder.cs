using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MagicEastern.CachedFunc
{
    class DictionaryCacheHolder<TKey, TValue> : ICacheHolder<TKey, TValue>
    {
        private Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        public bool TryGetValue(TKey key, int funcID, out TValue val)
        {
            return _dictionary.TryGetValue(key, out val);
        }

        public void Add(TKey key, int funcID, TValue val)
        {
            _dictionary[key] = val;
        }
    }
}
