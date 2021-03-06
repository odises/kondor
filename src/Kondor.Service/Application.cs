﻿using System;
using Kondor.Data.SettingModels;
using Kondor.Service.Handlers;
using Kondor.Service.Leitner;

namespace Kondor.Service
{
    public class Application : IApplication
    {
        private readonly ISettingHandler _settingHandler;
        private readonly ILeitnerService _leitnerService;
        private readonly ITelegramMessageHandler _telegramMessageHandler;
        private readonly INotificationHandler _notificationHandler;

        public Application(ISettingHandler settingHandler, ILeitnerService leitnerService, ITelegramMessageHandler telegramMessageHandler, INotificationHandler notificationHandler)
        {
            _settingHandler = settingHandler;
            _leitnerService = leitnerService;
            _telegramMessageHandler = telegramMessageHandler;
            _notificationHandler = notificationHandler;
        }

        public void Run()
        {
            var telegramTask = ObjectManager.GetInstance<ITaskManager>();
            telegramTask.Init(_settingHandler.GetSettings<GeneralSettings>().TelegramTaskInterval, TelegramJob);
            telegramTask.Start();

            var cleanerTask = ObjectManager.GetInstance<ITaskManager>();
            cleanerTask.Init(_settingHandler.GetSettings<GeneralSettings>().CleanUpTaskInterval, CleanerJob);
            cleanerTask.Start();

            var notificationTask = ObjectManager.GetInstance<ITaskManager>();
            notificationTask.Init(_settingHandler.GetSettings<GeneralSettings>().NotificationTaskInterval, NotificationJob);
            notificationTask.Start();
        }

        private void NotificationJob()
        {
            try
            {
                _notificationHandler.SendNotification();
            }
            catch (Exception exception)
            {
                Console.WriteLine("Notification:");
                Console.WriteLine(exception.Message);
            }
        }
        private void TelegramJob()
        {
            try
            {
                #if DEBUG
                _telegramMessageHandler.SaveUpdates();
                #endif

                _telegramMessageHandler.ProcessMessages();
            }
            catch (Exception exception)
            {
                Console.WriteLine("Telegram Message Handler:");
                Console.WriteLine(exception.Message);
            }
        }
        private void CleanerJob()
        {
            try
            {
                _leitnerService.BoxCleanUp();
            }
            catch (Exception exception)
            {
                Console.WriteLine("Cleaner:");
                Console.WriteLine(exception.Message);
            }
        }
    }
}
