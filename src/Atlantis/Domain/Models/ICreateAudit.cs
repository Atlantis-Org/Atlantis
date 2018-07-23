using System;

namespace Followme.AspNet.Core.FastCommon.Domain.Models
{
    public interface ICreateAudit
    {
        int CreateBy{get;set;}

        string CreateByName{get;set;}

        DateTime CreateTime{get;set;}
    }
}