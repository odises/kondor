﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Kondor.Data;
using Kondor.Data.DataModel;
using Kondor.Data.Enums;
using Kondor.Data.SettingModels;
using Kondor.Data.TelegramTypes;
using Kondor.Service.Managers;

namespace Kondor.Service.Handlers
{
    public class NotificationHandler : INotificationHandler
    {
        private readonly ITelegramApiManager _telegramApiManager;
        private readonly IDbContext _context;
        private readonly IUserApi _userApi;
        private readonly ISettingHandler _settingHandler;

        public NotificationHandler(ITelegramApiManager telegramApiManager, IDbContext context, IUserApi userApi, ISettingHandler settingHandler)
        {
            _telegramApiManager = telegramApiManager;
            _context = context;
            _userApi = userApi;
            _settingHandler = settingHandler;
        }

        public void SendNotification()
        {
            try
            {
                var maximumNumberOfAlert = _settingHandler.GetSettings<GeneralSettings>().MaximumNumberOfAlert;
                var alertsInterval = _settingHandler.GetSettings<GeneralSettings>().AlertsInterval;
                var userStateTolerance = _settingHandler.GetSettings<GeneralSettings>().UserStateTolerance;
                var removedMessagesText = _settingHandler.GetSettings<GeneralSettings>().RemovedMessagesText;

                var users = UsersThatShouldBeNotified(maximumNumberOfAlert, alertsInterval);

                var responseGroups = _context.Responses
                    .Where(p => p.Status == ResponseStatus.New)
                    .GroupBy(p => p.TelegramUserId).ToList();

                foreach (var responseGroup in responseGroups)
                {
                    var temp = responseGroup.FirstOrDefault();
                    if (temp != null)
                    {
                        if (users.Any(p => p.TelegramUserId == temp.TelegramUserId))
                        {
                            if (_userApi.GetUserState(temp.TelegramUserId, userStateTolerance) == UserState.Idle)
                            {
                                foreach (var response in responseGroup)
                                {
                                    _telegramApiManager.EditMessageText(response.ChatId, int.Parse(response.MessageId), removedMessagesText, "Markdown", true);

                                    response.Status = ResponseStatus.Removed;
                                    _context.Entry(response).State = EntityState.Modified;
                                }

                                var telegramUserId = temp.TelegramUserId;

                                _context.Notifications.Add(new Notification
                                {
                                    TelegramUserId = telegramUserId,
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
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        protected virtual List<ApplicationUser> UsersThatShouldBeNotified(int maximumAlert, int alertsInterval)
        {
            var datetime = DateTime.Now.AddHours(alertsInterval * -1);

            var query = from user in _context.Cards.Where(p => p.Status == CardStatus.NewInPosition && p.CardPosition != Position.Finished && p.ExaminationDateTime <= DateTime.Now)
                .GroupBy(p => p.UserId).Select(s => s.FirstOrDefault().User)
                        where
                            !_context.Notifications.Any(
                                p => p.TelegramUserId == user.TelegramUserId && p.CreationDatetime > datetime)
                        //&& _context.Notifications.Count(p => p.TelegramUserId == user.TelegramUserId) < maximumAlert
                        select user;

            var result = query.ToList();
            return result;
        }
    }
}
