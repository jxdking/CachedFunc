using System;

namespace MagicEastern.CachedFunc.Base
{
    public struct CacheKey<T> : IEquatable<CacheKey<T>>
    {
        public readonly int FuncID;
        public readonly T Value; // it should not be null

        public CacheKey(int funcID, T value)
        {
            FuncID = funcID;
            Value = value;
        }

        public bool Equals(CacheKey<T> other)
        {
            return Value.Equals(other.Value) && FuncID == other.FuncID;
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
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + Value.GetHashCode();
                hash = hash * 31 + FuncID.GetHashCode();
                return hash;
            }
        }
    }
}
