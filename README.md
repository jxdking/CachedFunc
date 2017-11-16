# CachedFunc

Provide a way to create a function that can cache results of previous runs. 

## Getting Started

For .Net Standard >= 1.3, get package from Nuget: [MagicEastern.CachedFunc.Core](https://www.nuget.org/packages/MagicEastern.CachedFunc.Core/)
For .Net Framework 4.5s, get package from Nuget: [MagicEastern.CachedFunc.Net45](https://www.nuget.org/packages/MagicEastern.CachedFunc.Net45/)

## How to Use

Assume there is a slow-running function.

```
static int SlowFunc(int n) {
    Thread.Sleep(1000);
    return n;
}
```
Create a CachedFuncSvc object. It will be used to create a cached function.

```
CachedFuncSvcBase CachedFunc = new CachedFuncSvc();
CachedFunc<int, int> cachedFunc = 
  CachedFunc.Create<int, int>(
    SlowFunc, 
    new CachedFuncOptions { 
      AbsoluteExpirationRelativeToNow = new TimeSpan(1, 0, 0) 
    }
  );
```

Then, you have a cached function. Call this function in the same way as the orignal function.

```
int result = cachedFunc(12345);
```
