using System;
using Kondor.Data.SettingModels;
using Kondor.Service.Handlers;
using Kondor.Service.Leitner;

namespace Kondor.Service
{
    public class Application : IApplication
    {
        private readonly INotificationHandler _notificationHandler;
        private readonly ITelegramMessageHandler _telegramMessageHandler;
        private readonly ILeitnerService _leitnerService;
        private readonly ISettingHandler _settingHandler;

        public Application(INotificationHandler notificationHandler, ITelegramMessageHandler telegramMessageHandler, ILeitnerService leitnerService, ISettingHandler settingHandler)
        {
            _notificationHandler = notificationHandler;
            _telegramMessageHandler = telegramMessageHandler;
            _leitnerService = leitnerService;
            _settingHandler = settingHandler;
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
            _notificationHandler.SendNotification();
            Console.WriteLine("Notification");
        }
        private  void TelegramJob()
        {
            try
            {
                _telegramMessageHandler.SaveUpdates();
                _telegramMessageHandler.ProcessMessages();
                Console.WriteLine("Telegram job");
            }
            catch (Exception exception)
            {
                Console.WriteLine("Telegram:");
                Console.WriteLine(exception.Message);
            }
        }
        private void CleanerJob()
        {
            try
            {
                var cleanerResult = _leitnerService.BoxCleanUp();
                Console.WriteLine(cleanerResult);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Cleaner:");
                Console.WriteLine(exception.Message);
            }
        }
    }
}
