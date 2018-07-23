using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Followme.AspNet.Core.FastCommon.CodeGeneration
{
    public class CodeAssembly
    {
        private readonly Assembly _assembly;

        internal CodeAssembly(Assembly assembly)
        {
            _assembly = assembly;
        }

        public Assembly Assembly => _assembly;
        
    }
}
