using System.Threading.Tasks;
using Followme.AspNet.Core.FastCommon.Infrastructure;

namespace Followme.AspNet.Core.FastCommon.Commanding
{
    public interface ICommandHandler
    {
        Task<TMessageResult> Handle<TMessage,TMessageResult>(TMessage message) where TMessageResult:MessageResult,new() where TMessage: BaseMessage;
        
        Task Handle<TMessage>(TMessage message) where TMessage: BaseMessage;

        Task<TMessageResult> SimpleExecuteMessageAsync<TMessage,TMessageResult>(TMessage message) where TMessageResult:MessageResult,new() where TMessage:BaseMessage;
    }
}
