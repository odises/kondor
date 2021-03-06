﻿using System;
using System.Collections.Generic;
using System.Linq;
using Kondor.Data.SettingModels;
using Kondor.Domain;
using Kondor.Domain.Enums;
using Kondor.Domain.Models;
using Kondor.Service.Leitner;
using Kondor.Service.Managers;

namespace Kondor.Service.Handlers
{
    public class NotificationHandler : INotificationHandler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITelegramApiManager _telegramApiManager;
        private readonly IUserApi _userApi;
        private readonly ISettingHandler _settingHandler;
        private readonly ITextManager _textManager;
        private readonly IViews _views;
        private readonly ILeitnerService _leitnerService;

        public NotificationHandler(ITelegramApiManager telegramApiManager, IUserApi userApi, ISettingHandler settingHandler, ITextManager textManager, IUnitOfWork unitOfWork, IViews views, ILeitnerService leitnerService)
        {
            _telegramApiManager = telegramApiManager;
            _userApi = userApi;
            _settingHandler = settingHandler;
            _textManager = textManager;
            _unitOfWork = unitOfWork;
            _views = views;
            _leitnerService = leitnerService;
        }

        public void SendNotification()
        {
            try
            {
                var maximumNumberOfAlert = _settingHandler.GetSettings<GeneralSettings>().MaximumNumberOfAlert;
                var alertsInterval = _settingHandler.GetSettings<GeneralSettings>().AlertsInterval;
                var durationToBeIdle = _settingHandler.GetSettings<GeneralSettings>().DurationToBeIdle;
                var removedMessagesText = _settingHandler.GetSettings<GeneralSettings>().RemovedMessagesText;

                var users = UsersThatShouldBeNotified(maximumNumberOfAlert, alertsInterval);

                var responseGroups = _unitOfWork.ResponseRepository.GetResponsesGroupedByTelegramUserId().ToList();

                foreach (var responseGroup in responseGroups)
                {
                    var temp = responseGroup.FirstOrDefault();
                    if (temp != null)
                    {
                        if (users.Any(p => p.TelegramUserId == temp.TelegramUserId))
                        {
                            if (_userApi.GetUserState(temp.TelegramUserId, durationToBeIdle) == UserState.Idle)
                            {
                                foreach (var response in responseGroup)
                                {
                                    try
                                    {
                                        _telegramApiManager.EditMessageText(response.ChatId, int.Parse(response.MessageId), removedMessagesText, "Markdown", true);
                                    }
                                    catch (Exception exception)
                                    {
                                        Console.WriteLine($"Notification {exception.Message}");
                                        // todo log critical
                                    }
                                    response.Status = ResponseStatus.Removed;
                                    _unitOfWork.ResponseRepository.Update(response);
                                }

                                var telegramUserId = temp.TelegramUserId;


                                var notification = new Notification
                                {
                                    TelegramUserId = telegramUserId,
                                    CreationDatetime = DateTime.Now
                                };

                                _unitOfWork.NotificationRepository.Insert(notification);

                                _unitOfWork.Save();


                                try
                                {
                                    var cardState = _leitnerService.GetCardForExam(telegramUserId);

                                    var position = string.Empty;
                                    for (var i = 0; i < (int)cardState.CardPosition + 1; i++)
                                    {
                                        position += _textManager.GetText(StringResources.Star);
                                    }

                                    var notificationMessageBody = $"{_textManager.GetText(StringResources.RemainingCards)} {_leitnerService.GetNumberOfCardsReadyToTry(telegramUserId)}\n\n{position}\n{cardState.Card.DeserializeCardData().GetFrontExamView()}";

                                    _telegramApiManager.SendMessage(temp.ChatId, notificationMessageBody, _views.Exam(cardState.Id).Keyboards);

                                }
                                catch (IndexOutOfRangeException)
                                {
                                    _telegramApiManager.SendMessage(temp.ChatId, _textManager.GetText(StringResources.BackMessage), _views.Index().Keyboards);
                                }
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
            var timeSpan = new TimeSpan(alertsInterval, 0, 0);

            var result = _unitOfWork.UserRepository.GetUsersThatShouldBeNotified(maximumAlert, timeSpan).ToList();
            return result;
        }
    }
}
