using System;
using System.Data.Entity;
using System.Linq;
using Kondor.Data;
using Kondor.Data.DataModel;
using Kondor.Data.Enums;
using Kondor.Data.TelegramTypes;
using Kondor.Service.Managers;

namespace Kondor.Service.Handlers
{
    public class NotificationHandler : INotificationHandler
    {
        private readonly ITelegramApiManager _telegramApiManager;
        private readonly IDbContext _context;

        public NotificationHandler(ITelegramApiManager telegramApiManager, IDbContext context)
        {
            _telegramApiManager = telegramApiManager;
            _context = context;
        }

        public void SendNotification()
        {

            var responseGroups = _context.Responses.Where(p => p.Status == ResponseStatus.New).GroupBy(p => p.ChatId).ToList();

            foreach (var group in responseGroups)
            {
                var temp = group.FirstOrDefault();

                foreach (var response in group)
                {
                    _telegramApiManager.EditMessageText(response.ChatId, int.Parse(response.MessageId), "\u2705", "Markdown", true);

                    response.Status = ResponseStatus.Removed;
                    _context.Entry(response).State = EntityState.Modified;
                }

                _context.Notifications.Add(new Notification
                {
                    ChatId = temp.ChatId,
                    CreationDatetime = DateTime.Now
                });

                _context.SaveChanges();

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
