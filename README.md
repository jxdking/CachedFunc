# CachedFunc 
**[Deprecated, use CachedFunc2 instead]**

Provide a way to create a function that can cache results of previous runs. 

## Getting Started

For .Net Standard >= 2.0 (including .Net Framework >= 4.6.1), get package from Nuget: [MagicEastern.CachedFunc.Core](https://www.nuget.org/packages/MagicEastern.CachedFunc.Core/)

For .Net Framework 4.5s, get package from Nuget: [MagicEastern.CachedFunc.Net45](https://www.nuget.org/packages/MagicEastern.CachedFunc.Net45/)

## How to Use

Assume there is a slow-running function.

```c#
static int SlowFunc(int n) {
    Thread.Sleep(1000);
    return n;
}
```
Starting from version 2.0.0, I added some extension method on Func<> delegate for ease of use. You can create a cached function from Func<> directly.

```c#
CachedFunc<int, int> cachedFunc = ((Func<int, int>)SlowFunc).ToCachedFunc(
    new CachedFuncOptions { 
        AbsoluteExpirationRelativeToNow = new TimeSpan(1, 0, 0) 
    }
)
```

Then, you have a cached function. Call this function in the same way as the orignal function.

```c#
int result = cachedFunc(12345);
```

See all available extension methods at [FuncExt.cs](https://github.com/jxdking/CachedFunc/blob/master/CachedFuncCore/FuncExt.cs)
