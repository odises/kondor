﻿using System;
using System.Timers;

namespace YourDictionary.Worker
{
    public class TelegramTask
    {
        private readonly Timer _timer;
        private readonly Action _action;

        /// <summary>
        /// Cleaner task
        /// </summary>
        /// <param name="interval">In miliseconds</param>
        /// <param name="method">An action that this task runs</param>
        public TelegramTask(int interval, Action method)
        {
            _timer = new Timer(interval);
            _timer.Elapsed += Timer_Elapsed;
            _action = method;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (this)
            {
                _action();
            }
        }

        public void Start()
        {
            _timer.Start();
        }
    }
}
