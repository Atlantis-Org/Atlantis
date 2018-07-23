using System;
using Followme.AspNet.Core.FastCommon.Utilities;
using Followme.AspNet.Core.FastCommon.Logging;
using Followme.AspNet.Core.FastCommon.Components;

namespace Followme.AspNet.Core.FastCommon.Scheduling
{
    public class Worker
    {
        private readonly string _workName;
        private readonly WorkOption _option;
        private readonly Action _action;
        private readonly ILogger _logger;
        private DateTime _lastUpdateTime;
        private int _scheduleInterval;
        
        public Worker(string workName, WorkCycle cycle, int interval, Action action)
        {
            Ensure.NotNull(workName, "The work name cannot be null!");
            Ensure.GrandThan(interval, 0, "The interval val cannot be less than zero!", false);
            
            _workName=workName;
            _option = new WorkOption(cycle,interval);
            _action=action;
            _logger=ObjectContainer.Resolve<ILoggerFactory>().Create(this.GetType());
            
            InitialingScheduleInterval();
        }

        public string Name=>_workName;

        public WorkOption Option=>_option;

        public int ScheduleInterval=>_scheduleInterval;
        
        public void Process()
        {
            try
            {
                _logger.Info($"The task is processing, task name: {_workName}");
                if(!EnterProcess())
                {
                    _logger.Info($"The task cycle not ending, task cannot be exexute! task name: {_workName}");
                    return;
                }
                _logger.Info($"The task will be execute! task name: {_workName}");
                _action();
                _logger.Info($"Execute action({_workName}) success, time is: {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}");
            }
            catch(Exception ex)
            {
                _logger.Info($"Execute action({_workName}) failed, the reason is: {ex.Message}, time is: {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}");
            }
            finally
            {
                ExitProcess();
            }
        }

        private void InitialingScheduleInterval()
        {
            switch(_option.Cycle)
            {
                case WorkCycle.Interval:_scheduleInterval=_option.Interval*1000;break;
                case WorkCycle.Minutes:_scheduleInterval=_option.Interval*1000*60;break;
                case WorkCycle.Hours:_scheduleInterval=1000*60*60;break;
                default:_scheduleInterval=1000*60*60*24;break;
            }
        }

        private bool EnterProcess()
        {
            switch(_option.Cycle)
            {
                case WorkCycle.Interval:
                case WorkCycle.Minutes:
                case WorkCycle.Hours:
                    if(_lastUpdateTime.AddHours(_option.Interval)<=DateTime.Now)return true;
                    else return false;
                case WorkCycle.Days:
                    if(_lastUpdateTime.AddDays(_option.Interval)<=DateTime.Now)return true;
                    else return false;
                case WorkCycle.Weeks:
                    var days=_option.Interval*7;
                    if(_lastUpdateTime.AddDays(days)<=DateTime.Now)return true;
                    else return false;
                case WorkCycle.Months:
                    if(_lastUpdateTime.AddMonths(_option.Interval)<=DateTime.Now)return true;
                    else return false;
                default:return false;
            }
        }

        private void ExitProcess()
        {
            _lastUpdateTime=DateTime.Now;
        }
    }

    public struct WorkOption
    {
        public WorkOption(WorkCycle cycle, int interval)
        {
            this.Cycle = cycle;
            this.Interval = interval;
        }

        public WorkCycle Cycle { get; set; }

        public int Interval { get; set; }
    }

    public enum WorkCycle
    {
        Interval,
        Minutes,
        Hours,
        Days,
        Weeks,
        Months
    }
}
