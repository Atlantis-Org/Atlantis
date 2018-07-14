using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Followme.AspNet.Core.FastCommon.Logging
{
    public class NullLoggerFactory : ILoggerFactory
    {
        public ILogger Create<T>()
        {
            return new NullLogger();
        }

        public ILogger Create(string name)
        {
            return new NullLogger();
        }

        public ILogger Create(Type type)
        {
            return new NullLogger();
        }
    }
}
