using System;
using System.Timers;

namespace YourDictionary.Worker
{
    public class CleanerTask
    {
        private readonly Timer _timer;
        private readonly Action _action;

        /// <summary>
        /// Cleaner task
        /// </summary>
        /// <param name="interval">In minutes</param>
        /// <param name="method">An action that this task runs</param>
        public CleanerTask(int interval, Action method)
        {
            _timer = new Timer(interval * 1000 * 60);
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
