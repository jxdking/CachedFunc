using MagicEastern.CachedFunc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Security.Cryptography;

namespace Test
{
    class Program
    {
        static CachedFuncSvc CachedFunc = new CachedFuncSvc();

        static int SomeFunc(int n)
        {
            for (int i = 0; i < 1000; i++)
            {
                n = n.GetHashCode();
            }
            return n;
        }

        static void VerifyResults(int[] res)
        {
            using (var hash = MD5.Create())
            {
                byte[] buf = new byte[sizeof(int) * res.Length];
                for (int i = 0; i < res.Length; i++)
                {
                    byte[] b = BitConverter.GetBytes(res[i]);
                    Array.Copy(b, 0, buf, i * sizeof(int), b.Length);
                }
                byte[] hashed = hash.ComputeHash(buf);
                string hex = BitConverter.ToString(hashed);
                Console.WriteLine($"Result's hash value: {hex}");
            }
        }

        static void Main(string[] args)
        {
            int arySize = 100000;
            int[] ary = new int[arySize];

            for (int i = 0; i < ary.Length; i++)
            {
                ary[i] = i;
            }

            BenchMarkCachedFunc<int, int>(SomeFunc, ary, VerifyResults);
            Console.ReadKey();
        }

        static void BenchMarkCachedFunc<T, TResult>(Func<T, TResult> func, T[] inputAry, Action<TResult[]> verifyFunc)
        {
            //var cachedFunc = CachedFunc.Create(func, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = new TimeSpan(1, 0, 0) });
            var cachedFunc = CachedFunc.Create(func);
            DateTime start;
            DateTime end;

            TResult[] res = new TResult[inputAry.Length];

            start = DateTime.Now;
            for (int i = 0; i < inputAry.Length; i++)
            {
                res[i] = func(inputAry[i]);
            }
            end = DateTime.Now;
            verifyFunc(res);
            Console.WriteLine($"Normal pass: {end.Subtract(start).TotalMilliseconds}ms");

            start = DateTime.Now;
            for (int i = 0; i < inputAry.Length; i++)
            {
                res[i] = cachedFunc(inputAry[i]);
            }
            end = DateTime.Now;
            verifyFunc(res);
            Console.WriteLine($"CachedFunc 1st pass: {end.Subtract(start).TotalMilliseconds}ms");

            start = DateTime.Now;
            for (int i = 0; i < inputAry.Length; i++)
            {
                res[i] = cachedFunc(inputAry[i]);
            }
            end = DateTime.Now;
            verifyFunc(res);
            Console.WriteLine($"CachedFunc 2st pass: {end.Subtract(start).TotalMilliseconds}ms");
        }
    }
}
