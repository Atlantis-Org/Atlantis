using System;
using Followme.AspNet.Core.FastCommon.Infrastructure;

namespace Followme.AspNet.Core.FastCommon.Commanding
{
    public interface ICommandServicerDelegateFactory
    {
        Func<TMessage,TMessageResult> GetHandleDelegate<TMessage,TMessageResult>(TMessage message) where TMessageResult :class where TMessage: BaseMessage;

        Action<TMessage> GetHandleDelegate<TMessage>(TMessage message)where TMessage: BaseMessage;
    }
}
