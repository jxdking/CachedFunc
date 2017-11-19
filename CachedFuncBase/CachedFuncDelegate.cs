using System;

namespace MagicEastern.CachedFunc
{
    /// <summary>
    /// Represent a cached function that taking no parameter.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="fallback">This will be called when cache is not found. If fallback is null, the default func that passed in CachedFuncBase.Create() will be called.</param>
    /// <param name="nocache"></param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">Thrown when cache value is not found and both fallback and func are null. func is the function that is passed in during CachedFuncBase.Create(). </exception>
    public delegate TResult CachedFunc<TResult>(Func<TResult> fallback = null, bool nocache = false);

    /// <summary>
    /// Represent a cached function that taking T type as a parameter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="input"></param>
    /// <param name="fallback">This will be called when cache is not found. If fallback is null, the default func that is passed in CachedFuncBase.Create() will be called.</param>
    /// <param name="nocache"></param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">Thrown when cache value is not found and both fallback and func are null. func is the function that is passed in during CachedFuncBase.Create(). </exception>
    public delegate TResult CachedFunc<T, TResult>(T input, Func<T, TResult> fallback = null, bool nocache = false);

    /// <summary>
    /// Represent a cached function that taking T type as a parameter. The internal cache use TKey type as the key, which is defined when the cached function is created.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="input"></param>
    /// <param name="fallback">This will be called when cache is not found. If fallback is null, the default func that passed in CachedFuncBase.Create() will be called.</param>
    /// <param name="nocache"></param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">Thrown when cache value is not found and both fallback and func are null. func is the function that is passed in during CachedFuncBase.Create(). </exception>
    public delegate TResult CachedFunc<T, TKey, TResult>(T input, Func<T, TResult> fallback = null, bool nocache = false);
}
