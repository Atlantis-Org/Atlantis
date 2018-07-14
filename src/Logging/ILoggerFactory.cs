using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Followme.AspNet.Core.FastCommon.Logging
{
    public interface ILoggerFactory
    {
        ILogger Create<T>();

        ILogger Create(string name);

        ILogger Create(Type type);
    }
}
