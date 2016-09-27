using System;
using Kondor.Service.Extensions;

namespace Kondor.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var datetime = DateTime.Now.AddHours(3);
            var temp1 = datetime.Humanize();
        }
    }
}
