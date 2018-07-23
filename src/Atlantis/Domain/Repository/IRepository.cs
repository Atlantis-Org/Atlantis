/*-------------------------------------------------------------------------
 * 作者：FRind
 * 创建时间： 2016/3/30 星期三 9:54:04
 * 版本号：v1.0
 * 本类主要用途描述：
 *  -------------------------------------------------------------------------*/

using Followme.AspNet.Core.FastCommon.Domain.Models;
using System.Threading.Tasks;

namespace Followme.AspNet.Core.FastCommon.Domain.Repository
{
    /// <summary>
    /// <see cref="IRepository"/>
    /// </summary>
    public interface IRepository<TAggregateRoot,TKey> where TAggregateRoot:IAggregateRoot<TKey>
    {
        Task<TAggregateRoot> GetById(TKey id);

        //TAggregateRoot Get(ISpecification<TAggregateRoot> predicate);

        //IEnumerable<TAggregateRoot> Get(ISpecification<TAggregateRoot> predicate, Expression<Func<TAggregateRoot, object>> orderBy, SortOrder sort = SortOrder.Asc);

        //IEnumerable<TAggregateRoot> GetAll(ISpecification<TAggregateRoot> predicate); 

        bool Add(TAggregateRoot aggregateRoot);

        bool Update(TAggregateRoot aggregateRoot);

        bool Delete(TAggregateRoot aggregateRoot);
    }

    public interface IRepository<TAggregateRoot> : IRepository<TAggregateRoot, int> where TAggregateRoot : IAggregateRoot
    { 
    
    }
}
