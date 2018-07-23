using System;
using Followme.AspNet.Core.FastCommon.Logging;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using ILogger=Followme.AspNet.Core.FastCommon.Logging.ILogger;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.Serilog
{
    public class SerilogLogger :ILogger
    {
        private readonly Logger _log;
        private readonly string _name;

        public SerilogLogger(string name,Logger log)
        {
            _name=name;
            _log=log;
        }

        public bool IsDebugEnabled =>_log.IsEnabled(LogEventLevel.Debug);

        public void Debug(string msg, Exception exception = null,object[] parameters=null)
        {
            _log.Debug(exception,Format(msg),parameters);
        }

        public void Error(string msg, Exception exception = null,object[] parameters=null)
        {
            _log.Error(exception,Format(msg),parameters);
        }

        public void Fatal(string msg, Exception exception = null,object[] parameters=null)
        {
            _log.Fatal(exception,Format(msg),parameters);
        }

        public void Info(string msg, Exception exception = null,object[] parameters=null)
        {
            _log.Information(exception,Format(msg),parameters);
        }

        public void Trace(string msg, Exception exception = null,object[] parameters=null)
        {
            Debug($"from trace {msg}",exception,parameters);
        }

        public void Warn(string msg, Exception exception = null,object[] parameters=null)
        {
            _log.Warning(exception,Format(msg),parameters);
        }

        private string Format(string msg)
        {
            return $"{msg} -> location: {_name}".Replace(@"""",@"\""").Replace(@"\\",@"\\\");
        }
    }
}
