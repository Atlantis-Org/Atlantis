/*-------------------------------------------------------------------------
 * 作者：FRind
 * 创建时间： 2016/3/30 星期三 10:07:52
 * 版本号：v1.0
 * 本类主要用途描述：
 *  -------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Followme.AspNet.Core.FastCommon.Domain.UnitOfWorks
{
    /// <summary>
    /// <see cref="IUnitOfWork"/>
    /// </summary>
    public interface IUnitOfWork:IDisposable
    {
        Guid Id { get; }

        bool IsCommited { get; }

        bool IsDisposed { get; }

        void Commit();

        void Rollback();
    }
}
