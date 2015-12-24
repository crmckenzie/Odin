using System;
using System.Collections.Generic;
using System.Linq;

namespace Odin.Configuration
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> SkipUntil<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source.SkipWhile(t => !predicate(t));
        }
        public static IEnumerable<T> TakeUntil<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source.TakeWhile(t => !predicate(t));
        }
    }
}