using System;
using System.Collections.Generic;
using System.Linq;

namespace Klootzakken.Server
{
    public static class EnumerableExtensions
    {
        public static T[] AsArray<T>(this IEnumerable<T> src) => src as T[] ?? src.ToArray();
        public static int FindSingle<T>(this IEnumerable<T> src, Predicate<T> which) => src.Select((item, i) => which(item) ? i : -1).Single(i => i >= 0);
        public static IEnumerable<T> TakeLast<T>(this ICollection<T> src, int count) => src.Skip(src.Count - count);
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> src, T item) => src.Concat(Enumerable.Repeat(item, 1));
    }
}