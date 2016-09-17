using System;
using System.Linq;
using System.Text;

namespace Kondor.Service.Extensions
{
    public static class StringExtensions
    {
        public static string GetBase64Encode(this string source)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(source);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string GetBase64Decode(this string source)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(source);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string ToBolder(this string sentence, string word)
        {
            var sequenceSimilarity = (from w in sentence.Split(new[] { ' ', ',', '.', '?', '!' }, StringSplitOptions.RemoveEmptyEntries)
                                      let percentage = (double)CalcLevenshteinDistance(word.ToLower(), w.ToLower()) / w.Length * 100
                                      select new Tuple<double, string>(percentage, w)).ToList();

            var result = sequenceSimilarity.Where(p => p.Item1 < 40).Select(p => p.Item2);

            var modifiedSentence = result.Aggregate(sentence, (current, item) => current.Replace(item, $"*{item}*"));

            return modifiedSentence;
        }

        private static int CalcLevenshteinDistance(string a, string b)
        {
            if (String.IsNullOrEmpty(a) || String.IsNullOrEmpty(b)) return 0;

            int lengthA = a.Length;
            int lengthB = b.Length;
            var distances = new int[lengthA + 1, lengthB + 1];
            for (int i = 0; i <= lengthA; distances[i, 0] = i++) ;
            for (int j = 0; j <= lengthB; distances[0, j] = j++) ;

            for (int i = 1; i <= lengthA; i++)
                for (int j = 1; j <= lengthB; j++)
                {
                    int cost = b[j - 1] == a[i - 1] ? 0 : 1;
                    distances[i, j] = Math.Min
                        (
                        Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                        distances[i - 1, j - 1] + cost
                        );
                }
            return distances[lengthA, lengthB];
        }
    }
}
