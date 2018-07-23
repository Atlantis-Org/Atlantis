using System;

namespace Followme.AspNet.Core.FastCommon.Domain.Models
{
    public interface IUpdateAudit
    {
         int? UpdateBy{get;set;}

         string UpdateByName{get;set;}

         DateTime? UpdateTime{get;set;}
    }
}