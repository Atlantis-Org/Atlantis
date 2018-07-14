using System.Threading.Tasks;
using Followme.AspNet.Core.FastCommon.Infrastructure;

namespace Followme.AspNet.Core.FastCommon.Commanding
{
    public interface ICommandHandler
    {
        TMessageResult Handle<TMessage,TMessageResult>(TMessage messsage)where TMessageResult:MessageResult ,new() where TMessage: BaseMessage;

        Task<TMessageResult> HandleAsync<TMessage,TMessageResult>(TMessage message) where TMessageResult:MessageResult,new() where TMessage: BaseMessage;

        void Handle<TMessage>(TMessage message) where TMessage: BaseMessage;

        Task HandleAsync<TMessage>(TMessage message) where TMessage: BaseMessage;

        Task<TMessageResult> SimpleExecuteMessageAsync<TMessage,TMessageResult>(TMessage message) where TMessageResult:MessageResult,new() where TMessage:BaseMessage;
    }
}
