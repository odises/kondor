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
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T GetRandom<T>(this IEnumerable<T> source)
        {
            var enumerable = source as IList<T> ?? source.ToList();
            var count = enumerable.Count;
            var rand = new Random();
            var randomNumber = rand.Next(0, count - 1);
            return enumerable.ElementAt(randomNumber);
        }
    }
}
