using System.IO;
using Followme.AspNet.Core.FastCommon.Serializing;
using ProtoBuf;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.Protobuf
{
    public class ProtobufBinarySerializer : IBinarySerializer
    {
        public T Deserialize<T>(byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
            {
                return Serializer.Deserialize<T>(memoryStream);
            }
        }

        public byte[] Serialize<T>(T obj)
        {
            using (var memoryStream = new MemoryStream())
            {
                Serializer.Serialize(memoryStream,obj);
               return memoryStream.ToArray();
            }
        }
    }
}