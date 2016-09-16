using System;
using System.Collections.Generic;
using System.Linq;

namespace Kondor.Service.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Gets a random element of source sequence
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TSource GetRandom<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                return default(TSource);
            }

            var enumerable = source as IList<TSource> ?? source.ToList();
            var count = enumerable.Count;
            if (count == 0)
            {
                return default(TSource);
            }
            var rand = new Random();
            var randomNumber = rand.Next(0, count - 1);
            return enumerable.ElementAt(randomNumber);
        }
    }
}
