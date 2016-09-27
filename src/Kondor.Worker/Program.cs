using System;
using Kondor.Service;
using Kondor.Service.Managers;

namespace YourDictionary.Worker
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
