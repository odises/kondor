using System;
using Kondor.Service.Leitner;

namespace Kondor.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            var leitnerCore = new LeitnerService(20, 10);

            Console.WriteLine(leitnerCore.BoxCleanUp());
        }
    }
}
