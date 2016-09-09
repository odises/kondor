using Kondor.Data.TelegramTypes;
using Newtonsoft.Json;

namespace Kondor.Service
{
    public class TelegramHelper
    {
        public static string GenerateReplyKeyboardMarkup(KeyboardButton[][] keyboard, bool resizeKeyboard, bool oneTimeKeyboard,
            bool selective)
        {
            var replyKeyboardMarkup = new ReplyKeyboardMarkup
            {
                Keyboard = keyboard,
                ResizeKeyboard = resizeKeyboard,
                OneTimeKeyboard = oneTimeKeyboard,
                Selective = selective
            };

            var serialized = JsonConvert.SerializeObject(replyKeyboardMarkup);
            return serialized;
        }

        public static string GetInlineKeyboardMarkup(InlineKeyboardButton[][] inlineKeyboard)
        {
            var inlineKeyboardMarkup = new InlineKeyboardMarkup {InlineKeyboard = inlineKeyboard};
            var serialized = JsonConvert.SerializeObject(inlineKeyboardMarkup, Formatting.None, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            return serialized;
        }
    }
}
