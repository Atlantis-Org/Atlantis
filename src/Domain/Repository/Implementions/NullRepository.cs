using System.Threading.Tasks;
using Followme.AspNet.Core.FastCommon.Domain.Models;

namespace Followme.AspNet.Core.FastCommon.Domain.Repository.Implementions
{
    public class NullRepository<TAggregateRoot> : IRepository<TAggregateRoot> where TAggregateRoot : IAggregateRoot,new()
    {
        public bool Add(TAggregateRoot aggregateRoot)
        {
            return false;
        }

        public bool Delete(TAggregateRoot aggregateRoot)
        {
            return false;
        }

        public Task<TAggregateRoot> GetById(int id)
        {
            return Task.FromResult(new TAggregateRoot());
        }

        public bool Update(TAggregateRoot aggregateRoot)
        {
           return false;
        }
    }
}
