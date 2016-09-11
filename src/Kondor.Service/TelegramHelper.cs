using System.Collections.Generic;
using System.Linq;
using Kondor.Data.TelegramTypes;
using Newtonsoft.Json;

namespace Kondor.Service
{
    public class TelegramHelper
    {
        public static string GenerateReplyKeyboardMarkup(params string[] keyboards)
        {
            return GenerateReplyKeyboardMarkup(new []
            {
                keyboards.Select(keyboard => new KeyboardButton
                {
                    Text = keyboard, RequestContact = false, RequestLocation = false
                }).ToArray()               
            }, false, false, false);
        }

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

            var serialized = replyKeyboardMarkup.ToJson();
            return serialized;
        }

        public static string GetInlineKeyboardMarkup(InlineKeyboardButton[][] inlineKeyboard)
        {
            var inlineKeyboardMarkup = new InlineKeyboardMarkup {InlineKeyboard = inlineKeyboard};
            var serialized = inlineKeyboardMarkup.ToJson();
            return serialized;
        }
    }
}
