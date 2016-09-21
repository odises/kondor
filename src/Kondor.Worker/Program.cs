using System;
using Kondor.Data.Enums;
using Kondor.Service;
using Kondor.Service.Leitner;

namespace YourDictionary.Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            var telegramTask = new TaskManager(800, TelegramJob);
            telegramTask.Start();

            var cleanerTask = new TaskManager(120000, CleanerJob);
            cleanerTask.Start();

            var notificationTask = new TaskManager(5000, NotificationJob);
            notificationTask.Start();

            Console.ReadLine();
        }
        private static void NotificationJob()
        {
            var notificationHandler = new NotificationHandler(new TelegramApiManager("bot264301717:AAHxLu9FcPWahQni6L8ahQvu74sHf-TlX_E"));
            notificationHandler.SendNotification();
            Console.WriteLine("Notification");
        }
        private static void TelegramJob()
        {
            try
            {
                var telegramMessageHandler = new TelegramMessageHandler(@"c:\test", "testkey", "http://www.kondor.com/account/newuser", new UserApi(), new LeitnerService(20, 15, TimeUnit.Minute), new TelegramApiManager("bot264301717:AAHxLu9FcPWahQni6L8ahQvu74sHf-TlX_E"));
                telegramMessageHandler.SaveUpdates();
                telegramMessageHandler.ProcessMessages();
                Console.WriteLine("Telegram job");
            }
            catch (Exception exception)
            {
                Console.WriteLine("Telegram:");
                Console.WriteLine(exception.Message);
            }
        }
        private static void CleanerJob()
        {
            try
            {
                var leitnerService = new LeitnerService(20, 15, TimeUnit.Minute);
                var cleanerResult = leitnerService.BoxCleanUp();
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
