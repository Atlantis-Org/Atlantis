using Followme.AspNet.Core.FastCommon.Components;
using Followme.AspNet.Core.FastCommon.Domain.Repository;
using Followme.AspNet.Core.FastCommon.Domain.UnitOfWorks;
using Followme.AspNet.Core.FastCommon.ThirdParty.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Followme.AspNet.Core.FastCommon.Configurations
{
    public static class EntityFrameworkCoreConfigurationExtension
    {
        public static Configuration RegisterEntityFrameworkRepositoryContext<TDbContext>(this Configuration configuration)where TDbContext:DbContext,new()
        {
            RepositoryContextManager.RegisterCreateNewContextFunc(()=>{return new EntityFrameworkCoreRepositoryContext<TDbContext>();});
            EntityFrameworkCoreDbContextFactory.RegisterDbContextDelegate(()=>{return new TDbContext();});
            return configuration;
        }
    }
}