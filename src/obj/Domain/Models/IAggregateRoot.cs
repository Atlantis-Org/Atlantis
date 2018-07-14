using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Followme.AspNet.Core.FastCommon.Domain.Models
{
    public interface IAggregateRoot: IAggregateRoot<int>
    {
    }

    public interface IAggregateRoot<TKey>:IEntity<TKey>
    { }
}
