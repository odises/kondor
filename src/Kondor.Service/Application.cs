using System;
using Kondor.Service.Handlers;
using Kondor.Service.Leitner;

namespace Kondor.Service
{
    public class Application : IApplication
    {
        private readonly INotificationHandler _notificationHandler;
        private readonly ITelegramMessageHandler _telegramMessageHandler;
        private readonly ILeitnerService _leitnerService;

        public Application(INotificationHandler notificationHandler, ITelegramMessageHandler telegramMessageHandler, ILeitnerService leitnerService)
        {
            _notificationHandler = notificationHandler;
            _telegramMessageHandler = telegramMessageHandler;
            _leitnerService = leitnerService;
        }

        public void Run()
        {
            var telegramTask = ObjectManager.GetInstance<ITaskManager>();
            telegramTask.Init(800, TelegramJob);
            telegramTask.Start();

            var cleanerTask = ObjectManager.GetInstance<ITaskManager>();
            cleanerTask.Init(120000, CleanerJob);
            cleanerTask.Start();

            var notificationTask = ObjectManager.GetInstance<ITaskManager>();
            notificationTask.Init(50000, NotificationJob);
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
                //var telegramMessageHandler = new TelegramMessageHandler("testkey", "http://www.kondor.com/account/newuser", new UserApi(), new LeitnerService(20, 15, TimeUnit.Minute), new TelegramApiManager("bot264301717:AAHxLu9FcPWahQni6L8ahQvu74sHf-TlX_E"), new List<Tuple<int, Card>>());
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
