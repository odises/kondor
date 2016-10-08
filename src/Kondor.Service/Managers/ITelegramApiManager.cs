using System;
using System.Collections.Generic;
using Kondor.Data.TelegramTypes;

namespace Kondor.Service.Managers
{
    public interface ITelegramApiManager
    {
        event EventHandler<MessageSentEventArgs> MessageSent;

        void AnswerCallbackQuery(string callbackQueryId, string text, bool showAlert, string url = null);
        void EditMessageText(string inlineMessageId, string text, string parseMode, bool disableWebPagePreview, string replyMarkup = null);
        void EditMessageText(int chatId, int messageId, string text, string parseMode, bool disableWebPagePreview, string replyMarkup = null);
        List<Update> GetUpdates(int? lastUpdateId = default(int?));
        Message SendMessage(int chatId, string text, string replyMarkup = null);
    }
}