using System.Collections.Generic;
using System.Collections.Immutable;

namespace NMaltParser.Utilities
{
    public class Collections
    {
        public static ImmutableSortedSet<T> SynchronizedSortedSet<T>(SortedSet<T> sortedSet)
        {
            return sortedSet.ToImmutableSortedSet();
        }

        public static ImmutableHashSet<T> SynchronizedSet<T>(HashSet<T> set)
        {
            return set.ToImmutableHashSet();
        }
    }
}