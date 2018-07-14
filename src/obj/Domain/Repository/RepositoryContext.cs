/*-------------------------------------------------------------------------
 * 作者：FRind
 * 创建时间： 2016/4/13 星期三 12:09:39
 * 版本号：v1.0
 * 本类主要用途描述：
 *  -------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Followme.AspNet.Core.FastCommon.Domain.UnitOfWorks;
using Followme.AspNet.Core.FastCommon.Domain.Models;
using System.Threading;
using Followme.AspNet.Core.FastCommon.Logging;
using Followme.AspNet.Core.FastCommon.Components;

namespace Followme.AspNet.Core.FastCommon.Domain.Repository
{
    /// <summary>
    /// <see cref="RepositoryContext"/>
    /// </summary>
    public abstract class RepositoryContext : IRepositoryContext
    {
        private ConcurrentQueue<EntityData> _operationQueue = new ConcurrentQueue<EntityData>();
        private volatile bool _isCommited;
        private Guid _id;
        protected bool _isDisposed;
        private readonly ILogger _logger;
        private int _dataLockForTrasactionOpened=0;

        public RepositoryContext()
        {
            _isCommited = false;
            _id = Guid.NewGuid();
            _isDisposed = false;
            _logger = ObjectContainer.Resolve<ILoggerFactory>().Create(GetType());
        }

        protected ConcurrentQueue<EntityData> OperationQueue => _operationQueue;

        protected bool IsDataLockTransactionEnabled => _dataLockForTrasactionOpened == 1 ? true : false;

        #region RepositoryContext Implements Method


        public virtual void RegisterAdded<TEntity>(TEntity entity) where TEntity : class,IEntity
        {
            _operationQueue.Enqueue(new EntityData(entity, OperationType.Insert));
        }

        public virtual void RegisterUpdated<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            _operationQueue.Enqueue(new EntityData(entity, OperationType.Modify));
        }

        public virtual void RegisterDeleted<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            _operationQueue.Enqueue(new EntityData(entity, OperationType.Remove));
        }

        public void DataLockForTrasaction()
        {
            if (Interlocked.CompareExchange(ref _dataLockForTrasactionOpened, 1, 0) != 0) return;
            DoDataLockForTrasaction();
        }

        #endregion

        public Guid Id
        {
            get { return _id; }
            private set { _id = value; }
        }

        public virtual bool IsCommited
        {
            get { return _isCommited; }
        }

        public bool IsDisposed
        {
            get { return _isDisposed; }
        }

        public void Commit()
        {
            try
            {
                DoCommit();
                _isCommited = true;
            }
            catch (Exception unitOfWorkException)
            {
                _logger.Error("提交出现错误！", unitOfWorkException);
                throw unitOfWorkException;
            }
        }

        public abstract void Rollback();

        public void Dispose()
        {
            if (_isDisposed) return;

            ClearRegistation();
            DoDispose(_isDisposed);
            _operationQueue = null;
            _isDisposed = true;
        }

        public void ChangeContextId(Guid newContextId)
        {
            if(Guid.Empty==newContextId) throw new ArgumentException("Change context id failed, new contextId is empty!");
            Id = newContextId;
        }

        protected abstract void DoDispose(bool dispose);

        protected abstract bool DoCommit();

        protected virtual void DoDataLockForTrasaction()
        { }

        protected void ClearRegistation()
        {
            EntityData useless;
            while (_operationQueue.TryDequeue(out useless))
            {
            }
        }

        public struct EntityData
        {
            public EntityData(IEntity entity,OperationType type)
            {
                Entity = entity;
                Type = type;
            }

            public IEntity Entity { get; }

            public OperationType Type { get; }
        }
    }

    public enum OperationType
    {
        Insert,
        Modify,
        Remove,
        UnChange
    }
}
