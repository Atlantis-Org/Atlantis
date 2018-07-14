using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Followme.AspNet.Core.FastCommon.Serializing
{
    public interface IJsonSerializer
    {
        string Serialize(object obj);

        T Deserialize<T>(string jsonStr);

        object Deserialize(string jsonStr);
    }
}
