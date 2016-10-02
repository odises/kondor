using System;
using Kondor.Service;

namespace Kondor.Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            ObjectManager.Initialize();

            var app = ObjectManager.GetInstance<IApplication>();

            app.Run();

            Console.ReadLine();
        }
    }
}
