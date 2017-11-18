using System;
namespace MagicEastern.CachedFunc
{
    public class CachedFuncOptions
    {
        public Nullable<DateTimeOffset> AbsoluteExpiration { get; set; }
        public Nullable<TimeSpan> AbsoluteExpirationRelativeToNow { get; set; }
        public Nullable<TimeSpan> SlidingExpiration { get; set; }
    }
}
