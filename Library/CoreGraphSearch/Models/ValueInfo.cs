using System.Collections.Generic;

namespace CoreGraphSearch.Models
{
    public struct ValueInfo<T>
    {
        public T Value { get; private set; }
        public int UniqueIndex { get; private set; }
        public bool IsIgnore { get; private set; }

        /// <param name="uniqueIndex">0 - int.MaxValue; при < 0 игнорируется информация</param>
        public ValueInfo(T value, int uniqueIndex)
        {
            Value = value;
            UniqueIndex = uniqueIndex;
            IsIgnore = UniqueIndex <= 0;
        }
        public ValueInfo(T value)
        {
            Value = value;
            UniqueIndex = -1;
            IsIgnore = true;
        }

        public static bool operator ==(ValueInfo<T> v1, ValueInfo<T> v2) => v1.UniqueIndex == v2.UniqueIndex;
        public static bool operator !=(ValueInfo<T> v1, ValueInfo<T> v2) => v1.UniqueIndex != v2.UniqueIndex;

        public override bool Equals(object obj)
        {
            return obj is ValueInfo<T> info&&
                   EqualityComparer<T>.Default.Equals(Value, info.Value)&&
                   UniqueIndex==info.UniqueIndex;
        }
        public override int GetHashCode() => UniqueIndex;
    }
}
