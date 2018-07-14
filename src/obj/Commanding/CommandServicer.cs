using System;
using System.Threading.Tasks;
using Followme.AspNet.Core.FastCommon.Components;
using Followme.AspNet.Core.FastCommon.Domain.UnitOfWorks;
using Followme.AspNet.Core.FastCommon.Infrastructure;

namespace Followme.AspNet.Core.FastCommon.Commanding
{
    public class CommandServicer
    {
        private readonly ICurrentRepositoryContextProvider _currentRepositoryContextProvider;
        private readonly IRepositoryContextManager _repositoryContextManager;
        private readonly ICommandHandler _commandHandler;
        private readonly ICommandServicerDelegateFactory _commandDelegateFactory;

        private object _sync = new object();

        public CommandServicer(ICurrentRepositoryContextProvider currentRepositoryContextProvider
            , IRepositoryContextManager repositoryContextManager, ICommandHandler commandHandler
            , ICommandServicerDelegateFactory commandDelegateFactory)
        {
            _currentRepositoryContextProvider = currentRepositoryContextProvider;
            _repositoryContextManager = repositoryContextManager;
            _commandHandler = commandHandler;
            _commandDelegateFactory = commandDelegateFactory;
        }

        protected async Task<TResult> RunChildrenCommandAsync<TMessage,TResult>(TMessage message,bool isImplContext=true)
            where TMessage:BaseMessage
            where TResult:MessageResult,new()
        {
            if(isImplContext) return await _commandHandler.SimpleExecuteMessageAsync<TMessage,TResult>(message);
            else return await _commandHandler.HandleAsync<TMessage,TResult>(message);
        }

        protected async Task RunChildrenCommandAsync<TMessage>(TMessage message,bool isImplContext=true)where TMessage:BaseMessage
        {
            if(isImplContext) await _commandHandler.SimpleExecuteMessageAsync<TMessage,MessageResult>(message);
            else await _commandHandler.HandleAsync(message);
        }

        /// <summary>
        /// Please use this method with caution!
        /// </summary>
        protected virtual void FlushToPersient()
        {
            lock(_sync)
            {
                var content=_currentRepositoryContextProvider.Current;
                var contextKey=content.Id;
                using(content)
                {
                    content.Commit();
                }
                _repositoryContextManager.Create();
                CurrentRepositoryContextProvider.ReplaceCurrentContextKey(contextKey);
            }
        }
    }
}
