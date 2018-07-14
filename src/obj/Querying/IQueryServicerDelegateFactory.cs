using System;
using Followme.AspNet.Core.FastCommon.Infrastructure;

namespace Followme.AspNet.Core.FastCommon.Querying
{
    public interface IQueryServicerDelegateFactory
    {
        Func<TMessage,TMessageResult> GetHandleDelegate<TMessage,TMessageResult>(TMessage message) where TMessageResult :class where TMessage: BaseMessage;
    }
}
