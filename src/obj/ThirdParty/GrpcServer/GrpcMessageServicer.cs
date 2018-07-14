using System;
using Followme.AspNet.Core.FastCommon.Infrastructure;
using Followme.AspNet.Core.FastCommon.ThirdParty.GrpcServer.Middlewares;
using Followme.AspNet.Core.FastCommon.Components;
using Grpc.Core;
using System.Threading.Tasks;
using Followme.AspNet.Core.FastCommon.Logging;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.GrpcServer
{
    public class GrpcMessageServicer
    {
        private readonly GrpcHandlerBuilder _builder;
        private readonly ILogger _logger;

        public GrpcMessageServicer()
        {
            _builder=ObjectContainer.Resolve<GrpcHandlerBuilder>();
            _logger=ObjectContainer.Resolve<ILoggerFactory>().Create<GrpcMessageServicer>();
        }
        
        public Task<TMessageResult> ProcessAsync<TMessage,TMessageResult>(TMessage message,ServerCallContext callContext)
            where TMessage:BaseMessage
            where TMessageResult:MessageResult,new()
        {
            return Task.Run(()=>
            {
                var context=new GrpcContext(message,callContext);
                _builder.DelegateProxy(context);
                if(context.Result is TMessageResult)
                {
                    return (TMessageResult)context.Result;  
                }
                else
                {
                    return new TMessageResult(){Code=context.Result.Code,Message=context.Result.Message};
                }
            });
        }
    }
}
