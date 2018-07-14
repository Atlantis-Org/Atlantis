using System;
using System.Data;
using Followme.AspNet.Core.FastCommon.Querying;
using MySql.Data.MySqlClient;
using Dapper;
using Followme.AspNet.Core.FastCommon.Components;
using Followme.AspNet.Core.FastCommon.Configurations;
using System.Linq;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.Dapper
{
    public class DapperHelper : IDisposable
    {
        private IDbConnection _dbConnection;
        private static QuerySetting _setting;

        public DapperHelper()
        {
            if(_setting==null)_setting=ObjectContainer.Resolve<Configuration>().GetSetting<QuerySetting>("QuerySetting");
        }

        public Pager<T> GetPager<T>(string strsql,string orderBy,int pageIndex,int pageSize,object parameter=null,int? timeout=null,string countSql=null)
        {
             if (string.IsNullOrWhiteSpace(strsql)) throw new ArgumentNullException("sql 语句不能为空！");
            if (string.IsNullOrWhiteSpace(orderBy)) throw new ArgumentNullException("orderby 列名不能为空！");
            if (pageIndex <= 0) throw new InvalidOperationException("Page index 必须大于零！");
            if (pageSize <= 0) throw new InvalidOperationException("Page size 必须大于零！");

            var pageStartIndex = pageIndex == 1 ? 0 : (pageIndex - 1) * pageSize;
            string pagingSql = $"{strsql} order by {orderBy} limit {pageStartIndex},{pageSize}";
            if(string.IsNullOrWhiteSpace(countSql)) countSql = $"select count(0) from ({strsql}) t";

            var list = DbConnection.Query<T>(pagingSql, parameter,commandTimeout:timeout).ToList();
            int totalRow = DbConnection.QueryFirst<int>(countSql, parameter);
            int totalPage=0;
            if(totalRow%pageSize==0)totalPage=totalRow/pageSize;
            else totalPage=(totalRow/pageSize)+1;
            return new Pager<T>(pageIndex,pageSize,totalPage,totalRow,list);
        }

        public IDbConnection DbConnection
        {
            get
            {
                if(_dbConnection==null)
                {
                    _dbConnection=new MySqlConnection(_setting.DbConnectString);
                    _dbConnection.Open();
                }
                return _dbConnection;
            }
        }

        public void Dispose()
        {
            if(_dbConnection==null)return;
            _dbConnection.Close();
            _dbConnection.Dispose();
            _dbConnection=null;
        }
    }
}
