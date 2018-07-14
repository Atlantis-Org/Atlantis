using Followme.AspNet.Core.FastCommon.CodeGeneration;
using Followme.AspNet.Core.FastCommon.Configurations;
using Followme.AspNet.Core.FastCommon.Infrastructure;
using Followme.AspNet.Core.FastCommon.Utilities;
using System.Text;
using System.Linq;
using System.Reflection;

namespace Followme.AspNet.Core.FastCommon.Commanding
{
    public static class CommandConfigurationExetension
    {
        internal static Configuration RegisteCommandServicer(this Configuration configuration)
        {
            var types=RefelectionHelper.GetImplInterfaceTypes(typeof(ICommandServicer),true,configuration.Setting.BussinessAssemblies);

            var codeClass=CodeBuilder.Instance.CreateClass("CommandServicerDelegateFactory",
                                    new string[]{"ICommandServicerDelegateFactory"},
                                    "Followme.AspNet.Core.FastCommon.Commanding")
                                    .AddRefence("using Followme.AspNet.Core.FastCommon.Components;",
                                                "using Followme.AspNet.Core.FastCommon.Infrastructure;",
                                                "using Followme.AspNet.Core.FastCommon.Commanding;",
                                                "using System.Threading.Tasks;");
            
            var needResult=new StringBuilder();
            var noResult=new StringBuilder();

            foreach(var type in types)
            {
                var methods=type.GetMethods();
                foreach(var method in methods)
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length != 1) continue;
                    if(method.ReturnType==typeof(void))
                    {
                        noResult.AppendLine($@"if(string.Equals(message.GetTypeFullName(),""{parameters[0].ParameterType.FullName}""))
                                    {{
                                        return async (m)=>await {{ObjectContainer.Resolve<{type.Name}>().{method.Name}(message as {parameters[0].ParameterType.FullName});}} ;   
                                    }}");
                    }
                    else
                    {
                        needResult.AppendLine($@"if(string.Equals(message.GetTypeFullName(),""{parameters[0].ParameterType.FullName}""))
                                    {{
                                        return async (m)=>{{return (await ObjectContainer.Resolve<{type.Name}>().{method.Name}(message as {parameters[0].ParameterType.FullName})) as TMessageResult;}} ;   
                                    }}");
                    }
                    CodeBuilder.Instance.AddAssemblyRefence(parameters[0].ParameterType.Assembly.Location);
                }
                codeClass.AddRefence($"using {type.Namespace};");
                CodeBuilder.Instance.AddAssemblyRefence(type.Assembly.Location);
            }

            noResult.Append("return null;");
            needResult.Append("return null;");

            codeClass.CreateMember("GetHandleDelegate<TMessage,TMessageResult>",
                    needResult.ToString(),
                    "Func<TMessage,Task<TMessageResult>>",
                    new CodeParameter[]{new CodeParameter("TMessage","message")},
                    new CodeMemberAttribute("public"),
                    "TMessageResult:class where TMessage:BaseMessage");
            codeClass.CreateMember("GetHandleDelegate<TMessage>",
                    noResult.ToString(),
                    "Func<TMessage,Task>",
                    new CodeParameter[]{new CodeParameter("TMessage","message")},
                    new CodeMemberAttribute("public"),
                    "TMessage:BaseMessage");
            
            var codeAssembly=CodeBuilder.Instance.AddAssemblyRefence(configuration.Setting.BussinessAssemblies.Select(p => p.Location).ToArray())
                .AddAssemblyRefence(Assembly.GetExecutingAssembly().Location);
            return configuration;
        }

        
    }
}
