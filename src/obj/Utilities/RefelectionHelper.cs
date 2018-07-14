using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace Followme.AspNet.Core.FastCommon.Utilities
{
    public class RefelectionHelper
    {
        public static IList<Type> GetImplInterfaceTypes(Type type,bool isInterfaceFilter=false, params Assembly[] assemblys)
        {
            var types = new List<Type>();
            foreach(var assembly in assemblys)
            {
              if(!isInterfaceFilter)types.AddRange(assembly.GetModules()[0].GetTypes().Where(p => p.GetInterface(type.Name)!=null));
              else types.AddRange(assembly.GetModules()[0].GetTypes().Where(p => p.GetInterface(type.Name) != null&&p.IsInterface));
            }
            return types;
        }
    }
}
