using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Followme.AspNet.Core.FastCommon.Components;
using Followme.AspNet.Core.FastCommon.Logging;
using Followme.AspNet.Core.FastCommon.ThirdParty.Serilog;
using Followme.AspNet.Core.FastCommon.Logging.Providers;

namespace Followme.AspNet.Core.FastCommon.Configurations
{
    public static class SerilogConfigurationExetension
    {
        public static Configuration UseSerilog(this Configuration configuration)
        {
            ObjectContainer.Register<ILoggerProvider, SerilogLoggerProvider>(LifeScope.Transient);
            ObjectContainer.Register<ILoggerFactory, SerilogLoggerFactory>();

            var providerSetting=new ProviderSetting()
            {
                FileName=$"normal-{ProviderSetting.FileNameDateSymbol}.log",
                FilePath="logs/"
            };
            ProviderSetting.SetDefault(providerSetting);
            
            return configuration;
        }
    }
}
