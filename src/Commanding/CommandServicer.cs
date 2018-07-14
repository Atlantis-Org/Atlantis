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
        private object _sync=new object();
        private ICommandHandler _commandHandler;
        private ICommandServicerDelegateFactory _commandDelegateFactory;

        public CommandServicer()
        {
            _currentRepositoryContextProvider=ObjectContainer.Resolve<ICurrentRepositoryContextProvider>();
            _repositoryContextManager=ObjectContainer.Resolve<IRepositoryContextManager>();
            _commandHandler=ObjectContainer.Resolve<ICommandHandler>();
            _commandDelegateFactory=ObjectContainer.Resolve<ICommandServicerDelegateFactory>();
        }

        [Obsolete("Please call RunCommandAsync Method.")]
        protected async Task<TResult> RunChildrenCommandAsync<TMessage,TResult>(TMessage message,bool isImplContext=true)
            where TMessage:BaseMessage
            where TResult:MessageResult,new()
        {
            return await RunCommandAsync<TMessage,TResult>(message,isImplContext);
        }

        [Obsolete("Please call RunCommandAsync Method.")]
        protected void RunChildrenCommandAsync<TMessage>(TMessage message,bool isImplContext=true)where TMessage:BaseMessage
        {
            RunCommandAsync(message,isImplContext);
        }

        protected async Task<TResult> RunCommandAsync<TMessage,TResult>(TMessage message,bool isImplContext=true)
            where TMessage:BaseMessage
            where TResult:MessageResult,new()
        {
            if(isImplContext) return await _commandHandler.SimpleExecuteMessageAsync<TMessage,TResult>(message);
            else return await _commandHandler.Handle<TMessage,TResult>(message);
        }

        protected async Task RunCommandAsync<TMessage>(TMessage message,bool isImplContext=true)where TMessage:BaseMessage
        {
            if(isImplContext) await _commandHandler.SimpleExecuteMessageAsync<TMessage,MessageResult>(message);
            else await _commandHandler.Handle(message);
        }

        protected Task RunWithNewContext(Func<Task> action)
        {
            return Task.Run(async ()=>
                    {
                        var context=_repositoryContextManager.Create();
                        await action();
                        _currentRepositoryContextProvider.Current.Commit();
                        _currentRepositoryContextProvider.Current.Dispose();
                        CurrentRepositoryContextProvider.ReleaseCurrentContext();
                    });
        }

        /// <summary>
        /// Please use this method with caution!
        /// </summary>
        [Obsolete("Please call FlushToPersistence method.")]
        protected void FlushToPersient()
        {
            FlushToPersistence();
        }

        protected void FlushToPersistence()
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
