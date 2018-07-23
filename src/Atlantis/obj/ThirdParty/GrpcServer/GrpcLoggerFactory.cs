using System;
using Followme.AspNet.Core.FastCommon.Logging;
using Followme.AspNet.Core.FastCommon.Logging.Providers;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.GrpcServer
{
    public interface IGrpcLoggerFactory:ILoggerFactory
    {
        
    }
    
    public class GrpcLoggerFactory:LoggerFactory,IGrpcLoggerFactory
    {
        public GrpcLoggerFactory(ILoggerProvider loggerProvider):base(loggerProvider)
        {
            
        }

        protected override ProviderSetting GetSetting()
        {
            var providerSetting=ProviderSetting.Default;
            providerSetting.FileName=string.Concat("access-",ProviderSetting.FileNameDateSymbol,".log");
            return providerSetting;
        }
    }
}
