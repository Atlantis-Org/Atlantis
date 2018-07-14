/*-------------------------------------------------------------------------
 * 作者：FRind
 * 创建时间： 2016/3/30 星期三 10:22:24
 * 版本号：v1.0
 * 本类主要用途描述：
 *  -------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Followme.AspNet.Core.FastCommon.Components;
using Followme.AspNet.Core.FastCommon.Domain.Repository;
using Followme.AspNet.Core.FastCommon.Domain.UnitOfWorks;
using Followme.AspNet.Core.FastCommon.Domain.Models;

namespace Followme.AspNet.Core.FastCommon.Domain.Repository
{
    /// <summary>
    /// <see cref="Repository"/>
    /// </summary>
    public abstract class Repository<TAggregateRoot, TKey,TRepostotyContext> : IRepository<TAggregateRoot, TKey> where TAggregateRoot:class,IAggregateRoot<TKey>
        where TRepostotyContext:IRepositoryContext
    {
        private readonly ICurrentRepositoryContextProvider _currentRepositoryContextProvider;

        public Repository()
        {
            _currentRepositoryContextProvider = ObjectContainer.Resolve<ICurrentRepositoryContextProvider>();
        }

        protected TRepostotyContext RepositoryContext { get { return (TRepostotyContext)_currentRepositoryContextProvider.Current; } }

        #region IRepository
        public TAggregateRoot GetById(TKey id)
        {
            return this.DoGetById(id);
        }

        public virtual bool Add(TAggregateRoot aggregateRoot)
        {
            _currentRepositoryContextProvider.Current.RegisterAdded( aggregateRoot);

            return true;
        }

        public virtual bool Update(TAggregateRoot aggregateRoot)
        {
            _currentRepositoryContextProvider.Current.RegisterUpdated( aggregateRoot);

            return true;
        }

        public virtual bool Delete(TAggregateRoot aggregateRoot)
        {
            _currentRepositoryContextProvider.Current.RegisterDeleted( aggregateRoot);

            return true;
        }
        #endregion
        
        protected abstract TAggregateRoot DoGetById(TKey id);
    }

    public abstract class Repository<TAggregateRoot, TRepositoryContext> : Repository<TAggregateRoot, int, TRepositoryContext> where TAggregateRoot : class,IAggregateRoot
        where TRepositoryContext:IRepositoryContext
    {
        public Repository()
        { }
    }
}
