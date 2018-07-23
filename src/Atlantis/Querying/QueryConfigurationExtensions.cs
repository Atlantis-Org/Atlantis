using System;
using System.Text;
using Followme.AspNet.Core.FastCommon.Utilities;
using Followme.AspNet.Core.FastCommon.CodeGeneration;
using Followme.AspNet.Core.FastCommon.Infrastructure;
using System.Linq;
using System.Reflection;

namespace Followme.AspNet.Core.FastCommon.Configurations
{
    public static class QueryConfigurationExtensions
    {
        internal static Configuration RegisteQueryServicer(this Configuration configuration)
        {
            var types=RefelectionHelper.GetImplInterfaceTypes(typeof(IQueryServicer),true,configuration.Setting.BussinessAssemblies);

            var codeClass=CodeBuilder.Instance.CreateClass("QueryServicerDelegateFactory",
                                    new string[]{"IQueryServicerDelegateFactory"},
                                    "Followme.AspNet.Core.FastCommon.Querying")
                                    .AddRefence("using Followme.AspNet.Core.FastCommon.Components;",
                                                "using System.Threading.Tasks;",
                                                "using Followme.AspNet.Core.FastCommon.Querying;");
            
            var needResult=new StringBuilder();

            foreach(var type in types)
            {
                var methods=type.GetMethods();
                foreach(var method in methods)
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length != 1) continue;
                    if(!method.ReturnType.Name.Contains("Task"))
                    {
                        throw new InvalidOperationException($"Please use async at method({type.FullName} -> {method.Name})");
                    }
                    
                    needResult.AppendLine($@"if(string.Equals(message.GetTypeFullName(),""{parameters[0].ParameterType.FullName}""))
                                {{
                                    return async (m)=>{{return (await ObjectContainer.Resolve<{type.Name}>().{method.Name}(message as {parameters[0].ParameterType.FullName})) as TMessageResult;}} ;
                                }}");
                    
                    CodeBuilder.Instance.AddAssemblyRefence(parameters[0].ParameterType.Assembly.Location);
                }
                codeClass.AddRefence($"using {type.Namespace};");
                CodeBuilder.Instance.AddAssemblyRefence(type.Assembly.Location);
            }

            needResult.Append("return null;");

            codeClass.CreateMember("GetHandleDelegateAsync<TMessage,TMessageResult>",
                    needResult.ToString(),
                    "Func<TMessage,Task<TMessageResult>>",               
                    new CodeParameter[]{new CodeParameter("TMessage","message")},
                    new CodeMemberAttribute("public"),
                    "TMessageResult:class where TMessage:BaseMessage");

            var codeAssembly=CodeBuilder.Instance.AddAssemblyRefence(configuration.Setting.BussinessAssemblies.Select(p => p.Location).ToArray())
                .AddAssemblyRefence(Assembly.GetExecutingAssembly().Location);
            return configuration;
        }

    }
}
