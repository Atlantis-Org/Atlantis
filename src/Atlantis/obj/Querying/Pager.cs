using System.Collections.Generic;

namespace Followme.AspNet.Core.FastCommon.Querying
{
    public class Pager<T>
    {
        public Pager(int pageIndex, int pageSize, int totalPage, int total,IList<T> items)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.TotalPage = totalPage;
            this.Total = total;
            this.Items=items;
        }
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int TotalPage { get; set; }

        public int Total { get; set; }

        public IList<T> Items { get; set; }
    }
}