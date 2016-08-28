using System;
using Kondor.Service.Leitner;

namespace Kondor.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            var leitnerCore = new LeitnerService(20, 10);
            //var word = leitnerCore.GetNewVocabulary(1);
            //Console.WriteLine(word.Vocabulary);
            Console.WriteLine(leitnerCore.BoxCleanUp());

            //var card = leitnerCore.GetCardForExam("odises");
            //leitnerCore.MoveNext(card);
            //Console.WriteLine(card.Word.Vocabulary);

            //8 * 60

            //2 * 24 * 60

        }
    }
}
