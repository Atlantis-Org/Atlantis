using System.Threading.Tasks;
using Followme.AspNet.Core.FastCommon.Domain.UnitOfWorks;
using Followme.AspNet.Core.FastCommon.Infrastructure;
using Followme.AspNet.Core.FastCommon.Logging;
using Followme.AspNet.Core.FastCommon.Serializing;
using Followme.AspNet.Core.FastCommon.Utilities;
using System;

namespace Followme.AspNet.Core.FastCommon.Commanding
{
    public class CommandHandler : ICommandHandler
    {
        private readonly IRepositoryContextManager _contextManager;
        private readonly ILogger _logger;
        private readonly IJsonSerializer _jsonSerialzer;
        private readonly ICurrentRepositoryContextProvider _currentContextProvider;
        private readonly ICommandServicerDelegateFactory _delegateFactory;

        public CommandHandler(IRepositoryContextManager contextManager, ILoggerFactory loggerFactory,
            IJsonSerializer jsonSerializer, ICurrentRepositoryContextProvider currentRepositoryContextProvider,
            ICommandServicerDelegateFactory delegateFactory)
        {
            _contextManager = contextManager;
            _logger = loggerFactory.Create<CommandHandler>();
            _jsonSerialzer = jsonSerializer;
            _currentContextProvider = currentRepositoryContextProvider;
            _delegateFactory = delegateFactory;
        }

        public TMessageResult Handle<TMessage, TMessageResult>(TMessage message) where TMessageResult : MessageResult, new() where TMessage: BaseMessage
        {
            return HandleAsync<TMessage, TMessageResult>(message).Result;
        }

        public void Handle<TMessage>(TMessage message)where TMessage:BaseMessage
        {
            HandleAsync(message).Wait();
        }

        public Task<TMessageResult> HandleAsync<TMessage, TMessageResult>(TMessage message) where TMessageResult : MessageResult, new() where TMessage: BaseMessage
        {
            return Task.Run(() =>
            {
                return Execute<TMessage, TMessageResult>(message);
            });
        }

        public Task HandleAsync<TMessage>(TMessage message)where TMessage: BaseMessage
        {
            return Task.Run(() =>
            {
                Execute<TMessage,MessageResult>(message);
            });
        }

        private TMessageResult Execute<TMessage, TMessageResult>(TMessage message, bool isNeedResult = true) where TMessageResult : MessageResult, new() where TMessage: BaseMessage
        {
            try
            {
                if (message == null) throw new ArgumentNullException("The message object is null!");
                if (_logger.IsDebugEnabled) _logger.Debug($"接收到消息{message.GetType()}，请求数据:{_jsonSerialzer.Serialize(message)}");

                _contextManager.Create();
                if (_logger.IsDebugEnabled) _logger.Debug($"创建新Context成功，ContextId: {_currentContextProvider.Current.Id}");
                var result = ExecuteMessage<TMessage, TMessageResult>(message);
                _currentContextProvider.Current.Commit();
                _currentContextProvider.Current.Dispose();
                if (_logger.IsDebugEnabled) _logger.Debug($"Context执行结束，ContextId: {_currentContextProvider.Current.Id}");
                CurrentRepositoryContextProvider.ReleaseCurrentContext(); 

                return result;
            }
            catch (Exception ex)
            {
                if(_currentContextProvider.Current!=null)
                {
                    _currentContextProvider.Current.Dispose();
                    CurrentRepositoryContextProvider.ReleaseCurrentContext();
                }
                var ensureException = ex;
                while (true)
                {
                    if (ensureException == null) break;
                    if (ensureException is EnsureException) break;
                    ensureException = ensureException.InnerException;
                }
                if (ensureException is EnsureException)
                {
                    _logger.Warn($"执行消息{message.GetType().Name}时业务失败，原因：{ensureException.Message}");
                    return new TMessageResult() { Code = ResultCode.BussinessError, Message = ensureException.Message };
                }
                else
                {
                    _logger.Fatal($"系统在执行{typeof(TMessage)}消息的时候出现异常！原因：{ex.GetExceptionMessage()}", ex);
                    return new TMessageResult() { Code = ResultCode.Exception, Message = ex.Message };
                }
            }
        }

        public async Task<TMessageResult> SimpleExecuteMessageAsync<TMessage, TMessageResult>(TMessage message) where TMessageResult : MessageResult, new() where TMessage: BaseMessage
        {
            return await Task.Run(()=>{return ExecuteMessage<TMessage,TMessageResult>(message);});
        }

        private TMessageResult ExecuteMessage<TMessage, TMessageResult>(TMessage message) where TMessageResult : MessageResult, new() where TMessage: BaseMessage
        {
            var func = _delegateFactory.GetHandleDelegate<TMessage, TMessageResult>(message);
            if (func != null) return func(message);
            var action = _delegateFactory.GetHandleDelegate<TMessage>(message);
            if (action == null) throw new ArgumentNullException($"Can not found message servicer delegate, please check it!");
            action(message);
            return (TMessageResult)new MessageResult() { Code = ResultCode.Success, Message = "no return func success" };
        }
    }
}
