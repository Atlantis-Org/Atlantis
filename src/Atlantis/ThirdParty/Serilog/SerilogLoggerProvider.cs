using System;
using Followme.AspNet.Core.FastCommon.Logging.Providers;
using Serilog.Core;
using Serilog;
using Serilog.Formatting.Compact;
using ILogger=Followme.AspNet.Core.FastCommon.Logging.ILogger;
using Serilog.Events;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.Serilog
{
    public class SerilogLoggerProvider:ILoggerProvider
    {
        private Logger _logger;
        
        public void Config(ProviderSetting setting)
        {
            _logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.RollingFile(new FastJsonTextFormatter(),
                                     string.Concat(setting.FilePath, setting.FileName),
                                     restrictedToMinimumLevel:LogEventLevel.Information,
                                     fileSizeLimitBytes:8589934592)
               .CreateLogger();
        }

        public ILogger CreateLogger(string name)
        {
            return new SerilogLogger(name, _logger);
        }
    }
}
