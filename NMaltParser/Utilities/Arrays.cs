using System;
using System.Collections.Generic;
using System.Linq;

namespace NMaltParser.Utilities
{
    public static class Arrays
    {
        public static T[] CopyOf<T>(IEnumerable<T> array, int newLength)
        {
            T[] enumerable = array as T[] ?? array.ToArray();

            if (enumerable.Length == newLength)
            {
                return new Span<T>(enumerable).ToArray();
            }
            else
            {
                Span<T> span = new T[newLength];

                (newLength < enumerable.Length ? new Span<T>(enumerable).Slice(0, newLength) : new Span<T>(enumerable)).CopyTo(span);

                return span.ToArray();
            }
        }
    }
}
