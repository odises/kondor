using System;
using System.Data.Entity;
using System.Linq;
using Kondor.Data;
using Kondor.Data.DataModel;
using Kondor.Data.Enums;
using Kondor.Data.TelegramTypes;

namespace Kondor.Service
{
    public class NotificationHandler
    {
        private readonly TelegramApiManager _telegramApiManager;

        public NotificationHandler(TelegramApiManager telegramApiManager)
        {
            _telegramApiManager = telegramApiManager;
        }

        public void SendNotification()
        {
            using (var entities = new EntityContext())
            {
                var responseGroups = entities.Responses.Where(p => p.Status == ResponseStatus.New).GroupBy(p => p.ChatId).ToList();

                foreach (var group in responseGroups)
                {
                    var temp = group.FirstOrDefault();

                    foreach (var response in group)
                    {
                        _telegramApiManager.EditMessageText(response.ChatId, int.Parse(response.MessageId), "\u2705", "Markdown", true);

                        response.Status = ResponseStatus.Removed;
                        entities.Entry(response).State = EntityState.Modified;
                    }

                    entities.Notifications.Add(new Notification
                    {
                        ChatId = temp.ChatId,
                        CreationDatetime = DateTime.Now
                    });

                    entities.SaveChanges();

                    _telegramApiManager.SendMessage(temp.ChatId, "What do you want to do?",
                        TelegramHelper.GetInlineKeyboardMarkup(new[]
                        {
                            new[]
                            {
                                new InlineKeyboardButton
                                {
                                    Text = "Learn",
                                    CallbackData = QueryData.NewQueryString("Learn", null, null)
                                },
                                new InlineKeyboardButton
                                {
                                    Text = "Exam",
                                    CallbackData = QueryData.NewQueryString("Exam", null, null)
                                }
                            }
                        }));
                }
            }
        }
    }
}
