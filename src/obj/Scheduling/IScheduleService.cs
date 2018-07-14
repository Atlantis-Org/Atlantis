﻿using System;

namespace Followme.AspNet.Core.FastCommon.Scheduling
{
    public interface IScheduleService
    {
        void StartTask(string name, Action action, int dueTime, int period);
        void StartTask(Worker worker);
        void StopTask(string name);
    }
}
