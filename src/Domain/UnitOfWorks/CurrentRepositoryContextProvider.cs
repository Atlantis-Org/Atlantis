/*-------------------------------------------------------------------------
 * 作者：FRind
 * 创建时间： 2016/3/30 星期三 15:29:11
 * 版本号：v1.0
 * 本类主要用途描述：
 *  -------------------------------------------------------------------------*/
 
using Followme.AspNet.Core.FastCommon.Domain.Repository;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Followme.AspNet.Core.FastCommon.Components;
using Followme.AspNet.Core.FastCommon.Logging;

namespace Followme.AspNet.Core.FastCommon.Domain.UnitOfWorks
{
    /// <summary>
    /// <see cref="CurrentRepositoryContextProvider"/>
    /// </summary>
    public class CurrentRepositoryContextProvider:ICurrentRepositoryContextProvider
    {
        // private static String ContextKey = "Followme.AspNet.Core.FastCommon.RepositoryContext.Current";

        private static ConcurrentDictionary<Guid, IRepositoryContext> _repositoryContextDictionary = new ConcurrentDictionary<Guid, IRepositoryContext>();

        private static AsyncLocal<Guid> _repositoryContextThreadLocal=new AsyncLocal<Guid>();

        private static ILogger _logger;

        private static object _sync=new object();

        public CurrentRepositoryContextProvider()
        {
        }

        public IRepositoryContext GetCurrentRepositoryContext()
        {
            var contextKey=_repositoryContextThreadLocal.Value;
            if(contextKey==null||contextKey==Guid.Empty)return null;

            IRepositoryContext context;
            if(!_repositoryContextDictionary.TryGetValue(contextKey, out context))return null;
            return context;
        } 

        public void SetCurrentRepositoryContext(IRepositoryContext repositoryContext)
        {
            lock(_sync)
            {
                if (repositoryContext == null || repositoryContext.IsDisposed)return;

                if(_repositoryContextDictionary.ContainsKey(repositoryContext.Id))
                {
                    Logger.Warn($"The repository context is confilt, context id: {repositoryContext.Id}");
                    _repositoryContextDictionary[repositoryContext.Id]=repositoryContext;
                }
                else
                {
                    _repositoryContextDictionary.TryAdd(repositoryContext.Id, repositoryContext);
                }
                _repositoryContextThreadLocal.Value=repositoryContext.Id;
            }
        }

        public IRepositoryContext Current{get=>GetCurrentRepositoryContext();set=>SetCurrentRepositoryContext(value);}

        protected static ILogger Logger
        {
            get
            {
                if(_logger==null)_logger=ObjectContainer.Resolve<ILoggerFactory>().Create<CurrentRepositoryContextProvider>();
                return _logger;
            }
        }

        internal static void ReplaceCurrentContextKey(Guid newKey)
        {
            lock(_sync)
            {
                if(newKey==null||newKey==Guid.Empty)throw new ArgumentException("The new key is empty, cannot replace current repositorycontext key ");
                var currentKey=_repositoryContextThreadLocal.Value;
                if(currentKey==null||currentKey==Guid.Empty)throw new ArgumentNullException("The current key is empty, replace new key failed!");
                IRepositoryContext currentContext;
                if(!_repositoryContextDictionary.TryGetValue(currentKey, out currentContext))throw new KeyNotFoundException("The current key cannot found current context, replace new key failed!");

                IRepositoryContext useless;
                _repositoryContextDictionary.TryRemove(currentKey, out useless);
                if(_repositoryContextDictionary.ContainsKey(newKey))
                {
                    _repositoryContextDictionary[newKey]=currentContext;
                }
                else
                {
                    _repositoryContextDictionary.TryAdd(newKey, currentContext);
                }

                _repositoryContextThreadLocal.Value = newKey;
                currentContext.ChangeContextId(newKey);
            }
        }
 
        public static void ReleaseCurrentContext()
        {
            lock (_sync)
            {
                var contextKey=_repositoryContextThreadLocal.Value;
                if(contextKey==null||contextKey==Guid.Empty)
                {
                    Logger.Error("Cannot release current context, cannot found repository key!");
                    return;
                }

                IRepositoryContext useless;
                _repositoryContextDictionary.TryRemove(contextKey, out useless);
                _repositoryContextThreadLocal.Value=Guid.Empty;   
            }
        }
    }
}
