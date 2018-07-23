using System;

namespace Followme.AspNet.Core.FastCommon.Logging.Providers
{
    public interface ILoggerProvider
    {
        void Config(ProviderSetting setting);

        ILogger CreateLogger(string name);
    }
}
