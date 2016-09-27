using System;
using Kondor.Data.SettingModels;
using Kondor.Service.Handlers;
using Kondor.Service.Leitner;

namespace Kondor.Service
{
    public class Application : IApplication
    {
        private readonly ISettingHandler _settingHandler;

        public Application(ISettingHandler settingHandler)
        {
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
            try
            {
                var notificationHandler = ObjectManager.GetInstance<INotificationHandler>();
                notificationHandler.SendNotification();
                Console.WriteLine("Notification");
            }
            catch (Exception exception)
            {
                Console.WriteLine("Notification:");
                Console.WriteLine(exception.Message);
            }
        }
        private  void TelegramJob()
        {
            try
            {
                var telegramMessageHandler = ObjectManager.GetInstance<ITelegramMessageHandler>();
                telegramMessageHandler.SaveUpdates();
                telegramMessageHandler.ProcessMessages();

                Console.WriteLine("T Job");
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
                var leitnerService = ObjectManager.GetInstance<ILeitnerService>();
                var cleanerResult = leitnerService.BoxCleanUp();

                Console.WriteLine("Cleaner: " + cleanerResult);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Cleaner:");
                Console.WriteLine(exception.Message);
            }
        }
    }
}
