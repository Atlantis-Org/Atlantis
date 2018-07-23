using Followme.AspNet.Core.FastCommon.Components;
using Followme.AspNet.Core.FastCommon.Domain.UnitOfWorks;

namespace Followme.AspNet.Core.FastCommon.Commanding
{
     public abstract class MessageServicer
    {
        private readonly ICurrentRepositoryContextProvider _provider;
        private readonly IRepositoryContextManager _repositoryContextManager;
        private object _sync = new object();

        public MessageServicer()
        {
            _provider = ObjectContainer.Resolve<ICurrentRepositoryContextProvider>();
            _repositoryContextManager = ObjectContainer.Resolve<IRepositoryContextManager>();
        }

        protected void FlushToPersistence()
        {
            lock (_sync)
            {
                IRepositoryContextManager contextManager = ObjectContainer.Resolve<IRepositoryContextManager>();
                using (var context = contextManager.Current)
                {
                    context.Commit();
                }

                contextManager.Create();
            }
        }
    }
}