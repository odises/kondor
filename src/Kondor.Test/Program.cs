using System;
using Kondor.Data.TelegramTypes;
using Kondor.Service;

namespace Kondor.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var telegramApiManager = new TelegramApiManager("bot264301717:AAHxLu9FcPWahQni6L8ahQvu74sHf-TlX_E");
            var message = telegramApiManager.SendMessage(42274705, "Hello", TelegramHelper.GetInlineKeyboardMarkup(new[]
            {
                new []
                {
                    new InlineKeyboardButton
                    {
                        Text = "Hello",
                        CallbackData = "callbackData"
                    }
                },
            }));
            telegramApiManager.EditMessageText(message.Chat.Id, int.Parse(message.MessageId), "*سلام حال خودت چطوری* عزیزم", "Markdown", true);
        }
    }
}
