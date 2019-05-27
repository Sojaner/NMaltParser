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

        public static int HashCode<T>(IEnumerable<T> array) where T:class => array?.Aggregate(1, (current, element) => 31 * current + (element?.GetHashCode() ?? 0)) ?? 0;

        public static bool Equals<T>(T[] array1, T[] array2) where T:IEquatable<T>
        {
            Span<T> span1 = array1;

            Span<T> span2 = array2;

            return span1.SequenceEqual(span2);
        }
    }
}
