using System;

namespace MagicEastern.CachedFunc
{
    public delegate TResult CachedFunc<TResult>(Func<TResult> fallback = null, bool nocache = false);
    public delegate TResult CachedFunc<T, TResult>(T input, Func<T, TResult> fallback = null, bool nocache = false);
    public delegate TResult CachedFunc<T, TKey, TResult>(T input, Func<T, TResult> fallback = null, bool nocache = false);
}
