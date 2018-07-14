using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Followme.AspNet.Core.FastCommon.Domain.Repository.Implementions
{
    public class NullRepositoryContext : RepositoryContext
    {

        public NullRepositoryContext()
        {
        }
        
        public override void Rollback()
        {
        }

        protected override bool DoCommit()
        {
            return true;
        }

        protected override void DoDispose(bool dispose)
        {
        }
    }
}
