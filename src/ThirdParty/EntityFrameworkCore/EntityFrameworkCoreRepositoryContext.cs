using Followme.AspNet.Core.FastCommon.Domain.Repository;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Followme.AspNet.Core.FastCommon.Logging;
using Followme.AspNet.Core.FastCommon.Components;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Concurrent;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.EntityFrameworkCore
{
    public class EntityFrameworkCoreRepositoryContext<TDbContext> : RepositoryContext where TDbContext : DbContext
    {
        private TDbContext _dbContext;
        private ILogger _logger;
        private ConcurrentQueue<ExecuteSqlInfo> _executeSqlQueue = new ConcurrentQueue<ExecuteSqlInfo>();
        private object _sync = new object();

        public TDbContext DbContext
        {
            get
            {
                if (_dbContext == null)
                {
                    lock (_sync)
                    {
                        if (_dbContext == null)
                        {
                            _dbContext = EntityFrameworkCoreDbContextFactory.CreateNewDbContext<TDbContext>();
                        }
                    }
                }
                return _dbContext;
            }
        }

        private ILogger Logger
        {
            get
            {
                if (_logger == null) _logger = ObjectContainer.Resolve<ILoggerFactory>().Create<EntityFrameworkCoreRepositoryContext<TDbContext>>();
                return _logger;
            }
        }

        public override void Rollback()
        {
        }

        protected override bool DoCommit()
        {
            if (!IsDataLockTransactionEnabled) DoDataLockForTrasaction();
            while (true)
            {
                ExecuteSqlInfo executeSqlInfo;
                if (!_executeSqlQueue.TryDequeue(out executeSqlInfo)) break;
                DbContext.Database.ExecuteSqlCommand(executeSqlInfo.Strsql, executeSqlInfo.Parameters);
                if (Logger.IsDebugEnabled) Logger.Debug($"执行sql语句：{executeSqlInfo.Strsql}");
            }
            DbContext.SaveChanges();
            if (DbContext.Database.CurrentTransaction != null)
            {
                DbContext.Database.CurrentTransaction.Commit();;
            }
            return true;
        }

        public override void RegisterAdded<TEntity>(TEntity entity)
        {
            DbContext.Set<TEntity>().Add(entity);
        }

        public override void RegisterUpdated<TEntity>(TEntity entity)
        {
            DbContext.Set<TEntity>().Update(entity);
        }

        public override void RegisterDeleted<TEntity>(TEntity entity)
        {
            DbContext.Set<TEntity>().Remove(entity);
        }

        public void ExecuteCommand(string strsql, params object[] parameters)
        {
            _executeSqlQueue.Enqueue(new ExecuteSqlInfo(strsql, parameters));
        }

        protected override void DoDataLockForTrasaction()
        {
            DbContext.Database.BeginTransaction();
        }

        protected override void DoDispose(bool dispose)
        {
            if (dispose) return;

            DbContext.Dispose();;
            ExecuteSqlInfo useless;
            while (_executeSqlQueue.TryDequeue(out useless)) ;
        }


        public async Task<TEntity> ToFind<TEntity>(params object[] ids) where TEntity : class
        {
            try
            {
                var result = await DbContext.Set<TEntity>().FindAsync(ids);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"获取数据库数据失败，执行Find方法获取对象{typeof(TEntity).Name}！", ex);
                return null;
            }
        }

        public async Task<TEntity> ToFirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            try
            { 
                var result = await DbContext.Set<TEntity>().FirstOrDefaultAsync(predicate);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"获取数据库数据失败，执行ToFirstOrDefault方法获取对象{typeof(TEntity).Name}！", ex);
                return null;
            }
        }

        public async Task<TEntity> ToFirstOrDefault<TEntity>(IQueryable<TEntity> queryable) where TEntity : class
        {
            try
            {
                var result =await queryable.FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"获取数据库数据失败，执行ToFirstOrDefault方法获取对象{typeof(TEntity).Name}！", ex);
                return null;
            }
        }

        public async Task<IList<TEntity>> ToAll<TEntity>(Expression<Func<TEntity, bool>> predicate = null) where TEntity : class
        {
            try
            {
                var result = predicate == null ? await DbContext.Set<TEntity>().ToListAsync() : await DbContext.Set<TEntity>().Where(predicate).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"获取数据库数据失败，执行ToAll方法获取对象{typeof(TEntity).Name}！", ex);
                return null;
            }
        }

        public async Task<IList<TEntity>> ToAll<TEntity>(IQueryable<TEntity> queryable) where TEntity : class
        {
            try
            {
                var result = await queryable.ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"获取数据库数据失败，执行ToAll方法获取对象{typeof(TEntity).Name}！", ex);
                return null;
            }
        }

        public async Task<int> ToCount<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            try
            {
                var result = await DbContext.Set<TEntity>().CountAsync(predicate);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"获取数据库数据失败，执行ToCount方法获取对象{typeof(TEntity).Name}！", ex);
                return 0;
            }
        }

        public async Task<IList<TEntity>> ToFromSql<TEntity>(string strsql, params object[] parameters) where TEntity : class
        {
            try
            {
                var result = await DbContext.Set<TEntity>().FromSql(strsql, parameters).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"获取数据库数据失败，执行ToFromSql方法获取对象{typeof(TEntity).Name}！", ex);
                return null;
            }
        }

        private class ExecuteSqlInfo
        {
            public ExecuteSqlInfo(string strsql, object[] parameters)
            {
                Strsql = strsql;
                Parameters = parameters;
            }

            public string Strsql { get; set; }

            public object[] Parameters { get; set; }
        }

    }
}
