using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Followme.AspNet.Core.FastCommon.Domain.Models
{
    public class Entity : IEntity
    {
    }

    public class Entity<TKey>:Entity,IEntity<TKey>
    {

    }
}
