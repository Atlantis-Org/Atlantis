/*-------------------------------------------------------------------------
 * 作者：FRind
 * 创建时间： 2016/4/13 星期三 12:06:38
 * 版本号：v1.0
 * 本类主要用途描述：
 *  -------------------------------------------------------------------------*/

using Followme.AspNet.Core.FastCommon.Domain.Models;
using Followme.AspNet.Core.FastCommon.Domain.UnitOfWorks;
using System;

namespace Followme.AspNet.Core.FastCommon.Domain.Repository
{
    /// <summary>
    /// <see cref="IRepositoryContext"/>
    /// </summary>
    public interface IRepositoryContext:IUnitOfWork
    {

        void RegisterAdded<TEntity>(TEntity entity) where TEntity : class, IEntity;

        void RegisterUpdated<TEntity>(TEntity entity) where TEntity : class, IEntity;

        void RegisterDeleted<TEntity>(TEntity entity) where TEntity : class, IEntity;

        void DataLockForTrasaction();

        void ChangeContextId(Guid newContextId);

    }
    
}
