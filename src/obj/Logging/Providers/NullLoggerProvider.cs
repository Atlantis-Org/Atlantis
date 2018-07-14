using System;

namespace Followme.AspNet.Core.FastCommon.Logging.Providers
{
    public class NullLoggerProvider:ILoggerProvider
    {
        public void Config(ProviderSetting setting)
        {
            return ;
        }

        public ILogger CreateLogger(string name)
        {
            return new NullLogger();
        }
    }
}
