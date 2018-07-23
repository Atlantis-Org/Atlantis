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

        public TAggregateRoot GetById(int id)
        {
            return new TAggregateRoot();
        }

        public bool Update(TAggregateRoot aggregateRoot)
        {
           return false;
        }
    }
}