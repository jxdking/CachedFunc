namespace MagicEastern.CachedFunc
{
    public interface ICacheHolder<TKey, TValue>
    {
        bool TryGetValue(TKey key, int funcID, out TValue val);
        void Add(TKey key, int funcID, TValue val);
    }
}
