using System;
using Kondor.Service;
using Kondor.Service.Extensions;

namespace Kondor.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var telegramApiManager = new TelegramApiManager("bot264301717:AAHxLu9FcPWahQni6L8ahQvu74sHf-TlX_E");
            var message = telegramApiManager.SendMessage(42274705, "This is just a test", TelegramHelper.GenerateReplyKeyboardMarkup("Hi", "Bye"));
            
        }
    }
}
