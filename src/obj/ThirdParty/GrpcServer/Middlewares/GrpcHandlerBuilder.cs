using System;
using Followme.AspNet.Core.FastCommon.ThirdParty.GrpcServer.Middlewares;
using System.Collections.Generic;
using Followme.AspNet.Core.FastCommon.Components;
using Followme.AspNet.Core.FastCommon.Infrastructure;
using System.Linq;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.GrpcServer.Middlewares
{
    public class GrpcHandlerBuilder
    {
        private readonly IList<Func<HandlerDelegate,HandlerDelegate>> _delegates;
        private readonly HandlerDelegate _last=
            (context)=>
            {
                context.HasDone=true;
                context.Result=new MessageResult(ResultCode.Exception,"No handler can be handle message!");
            };
        
        public GrpcHandlerBuilder()
        {
            _delegates=new List<Func<HandlerDelegate,HandlerDelegate>>();
        }

        public HandlerDelegate DelegateProxy{get;private set;}
        
        public GrpcHandlerBuilder UseMiddleware<T>()
        {
            var type=typeof(T);
            if(!typeof(GrpcMiddlewareBase).IsAssignableFrom(type))throw new InvalidCastException($"The middle haven't implement to GrpcMiddlewareBase! type is: {type.FullName}");

            return Use((next)=>{
                var constructor=type.GetConstructor(new[]{typeof(HandlerDelegate)});
                var middleware=(GrpcMiddlewareBase)constructor.Invoke(new object[]{next});
                return middleware.Handle;
            });
        }

        public GrpcHandlerBuilder Use(Func<HandlerDelegate,HandlerDelegate> func)
        {
            _delegates.Add(func); 
            return this;
        }

        public HandlerDelegate Build()
        {
            var handlerDelegate=_last;
            foreach(var delegateItem in _delegates.Reverse())
            {
                handlerDelegate= delegateItem(handlerDelegate);
            }
            DelegateProxy=handlerDelegate;
            return handlerDelegate;
        }
    }
}
