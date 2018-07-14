using System;
using System.Threading.Tasks;
using Followme.AspNet.Core.FastCommon.Infrastructure;

namespace Followme.AspNet.Core.FastCommon.Querying
{
    public interface IQueryServicerDelegateFactory
    {
        Func<TMessage,Task<TMessageResult>> GetHandleDelegateAsync<TMessage,TMessageResult>(TMessage message) where TMessageResult :class where TMessage: BaseMessage;
    }
}
