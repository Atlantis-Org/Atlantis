using System;
using System.Collections.Generic;
using System.Text;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.Cache
{
    public interface ICache
    {
        void Remove(string key);

        bool Set<T>(string key, T value, TimeSpan span);

        T Get<T>(string key);
    }
}
