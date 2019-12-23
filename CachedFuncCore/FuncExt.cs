using MagicEastern.CachedFunc.Core;
using System;

namespace MagicEastern.CachedFunc
{
    public static class FuncExt
    {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult">Type of return object of func</typeparam>
        /// <param name="func"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static CachedFunc<TResult> ToCachedFunc<TResult>(this Func<TResult> func, CachedFuncOptions options = null)
        {
            return CachedFuncSvc.Default.Create(func, options);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Type of input object of func</typeparam>
        /// <typeparam name="TResult">Type of return object of func</typeparam>
        /// <param name="func"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static CachedFunc<T, TResult> ToCachedFunc<T, TResult>(this Func<T, TResult> func, CachedFuncOptions options = null)
        {
            return CachedFuncSvc.Default.Create(func, options);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Type of input object of func</typeparam>
        /// <typeparam name="TKey">Type of return object of keySelector</typeparam>
        /// <typeparam name="TResult">Type of return object of func</typeparam>
        /// <param name="func"></param>
        /// <param name="keySelector">The result of keySelector will be use as key for internal Dictonary or MemoryCache object.</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static CachedFunc<T, TKey, TResult> ToCachedFunc<T, TKey, TResult>(this Func<T, TResult> func, Func<T, TKey> keySelector, CachedFuncOptions options = null)
        {
            return CachedFuncSvc.Default.Create(func, keySelector, options);
        }
    }
}
