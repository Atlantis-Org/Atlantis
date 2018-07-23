using Followme.AspNet.Core.FastCommon.Infrastructure;
using System;
using System.Threading.Tasks;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.GrpcServer.Middlewares
{
    public abstract class GrpcMiddlewareBase
    {
        private readonly HandlerDelegate _next;

        public GrpcMiddlewareBase(HandlerDelegate next)
        {
            _next=next;
        }

        public virtual void Handle(GrpcContext context)
        {
            DoHandle(context);
            if(!context.HasDone)_next(context);
            DoHandleResult(context);
        }

        protected abstract void DoHandle(GrpcContext context);

        protected virtual void DoHandleResult(GrpcContext context)
        {
        }
    }
}
