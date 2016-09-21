using System;

namespace Kondor.Service
{
    public interface ITaskManager
    {
        void Init(int interval, Action method);
        void Start();
    }
}