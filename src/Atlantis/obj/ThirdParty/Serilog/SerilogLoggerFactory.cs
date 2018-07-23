using System;
using Followme.AspNet.Core.FastCommon.Logging.Providers;

namespace Followme.AspNet.Core.FastCommon.Logging
{
    public class SerilogLoggerFactory:LoggerFactory
    {
        public SerilogLoggerFactory(ILoggerProvider loggerProvider):base(loggerProvider)
        {
        }

        protected override ProviderSetting GetSetting()
        {
            return ProviderSetting.Default;
        }
    }
}
