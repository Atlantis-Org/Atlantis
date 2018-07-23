using Followme.AspNet.Core.FastCommon.Domain.Models;
using  Followme.AspNet.Core.FastCommon.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.EntityFrameworkCore
{
    public class EntityFrameworkCoreRepository<TAggregateRoot,TDbContext> : Repository<TAggregateRoot, EntityFrameworkCoreRepositoryContext<TDbContext>>
    where TAggregateRoot:class,IAggregateRoot
    where TDbContext:DbContext
    {
        public EntityFrameworkCoreRepository()
        {
        }

        protected override async Task<TAggregateRoot> DoGetById(int id)
        {
            return await RepositoryContext.ToFind<TAggregateRoot>(id);
        }
    }

    public class EntityFrameworkCoreRepository<TAggregateRoot> : Repository<TAggregateRoot, EntityFrameworkCoreRepositoryContext<DbContext>>
    where TAggregateRoot:class,IAggregateRoot
    {
        public EntityFrameworkCoreRepository()
        {
        }

        protected override async Task<TAggregateRoot> DoGetById(int id)
        {
            return await RepositoryContext.ToFind<TAggregateRoot>(id);
        }
    }
}
