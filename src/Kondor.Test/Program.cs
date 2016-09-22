using System;
using System.Linq;

namespace Kondor.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(CalcLevenshteinDistance("entity", "entities"));
            var result =
                Bolder(
                    "Segmenting the business into three distinct entities should make it more attractive to buyers.",
                    "entity");
            Console.WriteLine(result);

            //var telegramApiManager = new TelegramApiManager("bot264301717:AAHxLu9FcPWahQni6L8ahQvu74sHf-TlX_E");
            //var message = telegramApiManager.SendMessage(42274705, "Hello", TelegramHelper.GetInlineKeyboardMarkup(new[]
            //{
            //    new []
            //    {
            //        new InlineKeyboardButton
            //        {
            //            Text = "Hello",
            //            CallbackData = "callbackData"
            //        }
            //    },
            //}));
            //telegramApiManager.EditMessageText(message.Chat.Id, int.Parse(message.MessageId), "*سلام حال خودت چطوری* عزیزم", "Markdown", true);
        }

        private static string Bolder(string sentence, string word)
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
