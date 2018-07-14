/*-------------------------------------------------------------------------
 * 作者：FRind
 * 创建时间： 2016/3/31 星期四 12:04:58
 * 版本号：v1.0
 * 本类主要用途描述：
 *  -------------------------------------------------------------------------*/

using Followme.AspNet.Core.FastCommon.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Followme.AspNet.Core.FastCommon.Domain.UnitOfWorks
{
    /// <summary>
    /// <see cref="RepositoryContextManager"/>
    /// </summary>
    public class RepositoryContextManager:IRepositoryContextManager
    {
        private ICurrentRepositoryContextProvider currentRepositoryContextProvider;
        private static Func<IRepositoryContext> _createNewContextFunc;

        public RepositoryContextManager(ICurrentRepositoryContextProvider currentRepositoryContextProvider)
        {
            this.currentRepositoryContextProvider = currentRepositoryContextProvider;
        }

        public IRepositoryContext Current
        {
            get
            {
                return currentRepositoryContextProvider.Current;
            }
        }

        public IRepositoryContext Create()
        {
            IRepositoryContext repositoryContext =_createNewContextFunc();
            this.currentRepositoryContextProvider.Current = repositoryContext;
            return repositoryContext;
        }

        public static void RegisterCreateNewContextFunc(Func<IRepositoryContext> createNewContextFunc)
        {
            _createNewContextFunc = createNewContextFunc;
        }

    }
}
