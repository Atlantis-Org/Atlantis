using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Followme.AspNet.Core.FastCommon.ThirdParty.RibbitMQ;

namespace Followme.AspNet.Core.FastCommon.Configurations
{
    public class ConfigurationSetting
    {
        public Assembly[] BussinessAssemblies { get; set; }

        public string BussinessConnectString { get; set; }

        internal Assembly SysAssembly{get;set;}
    }
}
