using System;
using System.Threading.Tasks;
using Followme.AspNet.Core.FastCommon.Infrastructure;

namespace Followme.AspNet.Core.FastCommon.Commanding
{
    public interface ICommandServicerDelegateFactory
    {
        Func<TMessage,Task<TMessageResult>> GetHandleDelegate<TMessage,TMessageResult>(TMessage message) where TMessageResult :class where TMessage: BaseMessage;

        Func<TMessage,Task> GetHandleDelegate<TMessage>(TMessage message)where TMessage: BaseMessage;
    }
}
