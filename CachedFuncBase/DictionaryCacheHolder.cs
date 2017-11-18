using System.Collections.Concurrent;

namespace MagicEastern.CachedFunc
{
    class DictionaryCacheHolder<TKey, TValue> : ICacheHolder<TKey, TValue>
    {
        private ConcurrentDictionary<TKey, TValue> _dictionary = new ConcurrentDictionary<TKey, TValue>();


        public bool TryGetValue(TKey key, int funcID, out TValue val) {
            return _dictionary.TryGetValue(key, out val);
        }

        public void Add(TKey key, int funcID, TValue val) {
            _dictionary.TryAdd(key, val);
        }
    }
}
