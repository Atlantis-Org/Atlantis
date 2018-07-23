using Followme.AspNet.Core.FastCommon.Components;
using Followme.AspNet.Core.FastCommon.Serializing;
using Followme.AspNet.Core.FastCommon.ThirdParty.Newtonsoft;

namespace Followme.AspNet.Core.FastCommon.Configurations
{
    public static class NewtonsoftJsonConfigurationExetension
    {
        public static Configuration UseNewtonsoftJson(this Configuration configuration)
        {
            ObjectContainer.Register<IJsonSerializer,NewtonsoftJsonSerializer>();
            return configuration;
        }
    }
}