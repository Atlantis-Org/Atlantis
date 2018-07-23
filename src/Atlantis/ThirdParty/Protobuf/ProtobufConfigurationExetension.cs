using Followme.AspNet.Core.FastCommon.Components;
using Followme.AspNet.Core.FastCommon.Serializing;
using Followme.AspNet.Core.FastCommon.ThirdParty.Protobuf;

namespace Followme.AspNet.Core.FastCommon.Configurations
{
    public static class ProtobufConfigurationExetension
    {
        public static Configuration UseProtobufSerializer(this Configuration configuration)
        {
            ObjectContainer.Register<IBinarySerializer,ProtobufBinarySerializer>();
            return configuration;
        }
    }
}