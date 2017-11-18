using System;

namespace MagicEastern.CachedFuncCore
{
    struct CacheKey<T> : IEquatable<CacheKey<T>>
    {
        public int FuncID;
        public T Value; // not null

        public CacheKey(int funcID, T value)
        {
            FuncID = funcID;
            Value = value;
        }

        public bool Equals(CacheKey<T> other)
        {
            if (FuncID != other.FuncID) {
                return false;
            }
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CacheKey<T>)) {
                return false;
            }
            return Equals((CacheKey<T>)obj); 
        }

        public override int GetHashCode()
        {
            unchecked {
                return FuncID.GetHashCode() + Value.GetHashCode();
            }
            
        }
    }
}
