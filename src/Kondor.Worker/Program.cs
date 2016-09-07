﻿using System;
using Kondor.Service;
using Kondor.Service.Leitner;

namespace YourDictionary.Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            var telegramTask = new TelegramTask(500, TelegramJob);
            telegramTask.Start();

            var cleanerTask = new CleanerTask(10, CleanerJob);
            cleanerTask.Start();

            Console.ReadLine();
        }

        private static void TelegramJob()
        {
            try
            {
                var telegramMessageHandler = new TelegramMessageHandler("bot264301717:AAHxLu9FcPWahQni6L8ahQvu74sHf-TlX_E", @"c:\test", "testkey", "http://yahoo.com/account/newuser");
                telegramMessageHandler.GetMessages();
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
                var leitnerService = new LeitnerService(20, 15);
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
