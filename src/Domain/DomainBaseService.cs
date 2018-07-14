using System;
using System.Threading.Tasks;
using Followme.AspNet.Core.FastCommon.Components;
using Followme.AspNet.Core.FastCommon.Domain.UnitOfWorks;

namespace Followme.AspNet.Core.FastCommon.Domain
{
    public class DomainBaseService
    {
        private readonly ICurrentRepositoryContextProvider _currentRepositoryContextProvider;
        private readonly IRepositoryContextManager _repositoryContextManager;
        private object _sync=new object();

        public DomainBaseService()
        {
            _currentRepositoryContextProvider=ObjectContainer.Resolve<ICurrentRepositoryContextProvider>();
            _repositoryContextManager=ObjectContainer.Resolve<IRepositoryContextManager>();
        }

        
        protected void FlushToPersistence()
        {
            lock(_sync)
            {
                var content=_currentRepositoryContextProvider.Current;
                var contextKey=content.Id;
                if(content==null||content.IsDisposed)throw new ObjectDisposedException($"对象已被释放！ID: {contextKey}");
                using(content)
                {
                    content.Commit();
                }
                _repositoryContextManager.Create();
                CurrentRepositoryContextProvider.ReplaceCurrentContextKey(contextKey);
            }
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
    }
}
