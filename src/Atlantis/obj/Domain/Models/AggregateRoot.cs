using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Followme.AspNet.Core.FastCommon.Utilities;

namespace Followme.AspNet.Core.FastCommon.Domain.Models
{
    public class AggregateRoot: AggregateRoot<int>, IAggregateRoot
    {
        public void AddDeleteTag(int operatorUserId,string operatorUserName)
        {
            var deleteEntity=this as ISoftDelete;
            if(deleteEntity!=null)deleteEntity.IsDelete=true;
            var updateEntity=this as IUpdateAudit;
            if(updateEntity!=null)
            {
                Ensure.GrandThan(operatorUserId,0,"操作人信息不正确！",false);
                updateEntity.UpdateBy=operatorUserId;
                updateEntity.UpdateByName=operatorUserName;
                updateEntity.UpdateTime=DateTime.Now;
            }
        }
    }

    public class AggregateRoot<TKey>:Entity<TKey>,IAggregateRoot<TKey>
    {

    }
}
