# CachedFunc

Provide a way to create a function that can cache results of previous runs. 

## Getting Started

Get package from Nuget: [MagicEastern.CachedFunc.Core](https://www.nuget.org/packages/MagicEastern.CachedFunc.Core/)


## How to Use

Assume there is a slow running function.

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

Then, you have a cached function. .

```
int result = cachedFunc(12345);
```
