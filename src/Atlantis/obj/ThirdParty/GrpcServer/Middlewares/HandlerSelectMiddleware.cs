using System;
using Followme.AspNet.Core.FastCommon.Commanding;
using Followme.AspNet.Core.FastCommon.Querying;
using Followme.AspNet.Core.FastCommon.Components;
using Followme.AspNet.Core.FastCommon.Infrastructure;
using System.Threading.Tasks;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.GrpcServer.Middlewares
{
    public class HandlerSelectMiddleware:GrpcMiddlewareBase
    {
        private readonly ICommandHandler _commandHandler;
        private readonly IQueryServicerDelegateFactory _queryDelegateFactory;
        
        public HandlerSelectMiddleware(HandlerDelegate next):base(next)
        {
            _commandHandler=ObjectContainer.Resolve<ICommandHandler>();
            _queryDelegateFactory=ObjectContainer.Resolve<IQueryServicerDelegateFactory>();
        }

        protected override void DoHandle(GrpcContext context)
        {
            switch(context.Message.GetMessageExecutingType())
            {
                case MessageExecutingType.Command:HandleCommand(context).Wait();return;
                case MessageExecutingType.Query:HandleQuery(context).Wait();return;
            }
        }

        private async Task HandleCommand(GrpcContext context)
        {
            context.Result=await _commandHandler.HandleAsync<BaseMessage,MessageResult>(context.Message);
            context.HasDone=true;
        }

        private async Task HandleQuery(GrpcContext context)
        {
            context.Result=await Task.Run(()=>{return _queryDelegateFactory.GetHandleDelegate<BaseMessage,MessageResult>(context.Message)(context.Message);});
            context.HasDone=true;
        }
    }
}
