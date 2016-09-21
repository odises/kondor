using System;
using System.Timers;

namespace Kondor.Service
{
    public class TaskManager : ITaskManager
    {
        private Timer _timer;
        private Action _action;

        public void Init(int interval, Action method)
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
