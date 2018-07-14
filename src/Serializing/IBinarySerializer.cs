namespace Followme.AspNet.Core.FastCommon.Serializing
{
    public interface IBinarySerializer
    {
         byte[] Serialize<T>(T obj);

         T Deserialize<T>(byte[] data);
    }
}