using System;
using Microsoft.EntityFrameworkCore;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.EntityFrameworkCore
{
    public static class EntityFrameworkCoreDbContextFactory
    {
        private static Func<DbContext> _dbContextDelegate;

        public static void RegisterDbContextDelegate(Func<DbContext> dbContextDelegate)
        {
            _dbContextDelegate = dbContextDelegate;
        }

        public static TDbContext CreateNewDbContext<TDbContext>()where TDbContext:DbContext
        {
            return (TDbContext)_dbContextDelegate();
        }
    }
}