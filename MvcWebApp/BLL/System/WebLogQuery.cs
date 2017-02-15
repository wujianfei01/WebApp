using DAL.Interface;
using LoggerPlugin;
using MODEL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Util;

namespace BLL.System
{
    public class WebLogQuery
    {
        /// <summary>
        /// webconfig中的連接字符串key
        /// </summary>
        string connKey01 = "ConnectionStringOracle";

        private int pageSize;
        private int pageIndex;
        public WebLogQuery()
        {
            pageSize = 10;
            pageIndex = 1;
        }

        public DataTable GetAllLog()
        {
            LogHelper.WriteSqlDbLogForWarn("用户[" + UserInfo.EmpNo + "]" + UserInfo.EUserName + "下载了网站日志!");
            string sql = "SELECT DATETIME,THREAD,LOG_LEVEL,LOGGER,MESSAGE FROM F_LOG ";
            //為sqlserver數據庫初始化dal
            IDal iDal = IDal.InstanceOracle(connKey01);

            //取得數據
            return iDal.QuerySql(sql) != null ? iDal.QuerySql(sql).Tables[0] : null;
        }
        public JsonData Query(FormCollection collection, string _pageIndex, string _pageSize)
        {
            string sql = "SELECT DATETIME,THREAD,LOG_LEVEL,LOGGER,MESSAGE FROM F_LOG WHERE 1 = 1 ";
            List<object> lps = new List<object>();
            if (!string.IsNullOrEmpty(collection["logmsg"]))
            {
                sql += "AND UPPER(MESSAGE) LIKE :MESSAGE";
                OracleParameter spmenuname = new OracleParameter(":MESSAGE", "%" + collection["logmsg"].ToUpper() + "%");
                lps.Add(spmenuname);
            }
            sql += " ORDER BY DATETIME DESC";

            //取得分頁信息
            int.TryParse(_pageIndex, out pageIndex);
            int.TryParse(_pageSize, out pageSize);

            //防止分頁異常
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageIndex = pageIndex < 1 ? 1 : pageIndex;

            //為sqlserver數據庫初始化dal
            IDal iDal = IDal.InstanceOracle(connKey01);

            //取得數據
            DataTable dt = iDal.QuerySqlWithParameters(sql, lps) != null ? iDal.QuerySqlWithParameters(sql, lps).Tables[0] : null;
            //取得數據總數
            int totalRows = dt != null ? dt.Rows.Count : 0;
            //數據分頁:通用化,避免數據庫差異帶來分頁問題
            return new JsonData() { total = totalRows, rows = DataTableHelper.GetPagedTable(dt, pageIndex, pageSize) };
        }
    }

    public class WebLogQueryForSQLServer
    {
        /// <summary>
        /// webconfig中的連接字符串key
        /// </summary>
        string connKey01 = "ConnectionStringSqlServer";

        private int pageSize;
        private int pageIndex;
        public WebLogQueryForSQLServer()
        {
            pageSize = 10;
            pageIndex = 1;
        }

        public JsonData Query(FormCollection collection, string _pageIndex, string _pageSize)
        {
            string sql = "SELECT DATETIME,THREAD,LOG_LEVEL,LOGGER,MESSAGE FROM F_LOG WHERE 1 = 1 ";
            List<object> lps = new List<object>();
            if (!string.IsNullOrEmpty(collection["logmsg"]))
            {
                sql += "AND UPPER(MESSAGE) LIKE @MESSAGE";
                SqlParameter spmenuname = new SqlParameter("@MESSAGE", "%" + collection["logmsg"].ToUpper() + "%");
                lps.Add(spmenuname);
            }
            sql += " ORDER BY DATETIME DESC";
            //取得分頁信息
            int.TryParse(_pageIndex, out pageIndex);
            int.TryParse(_pageSize, out pageSize);

            //防止分頁異常
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageIndex = pageIndex < 1 ? 1 : pageIndex;

            //為sqlserver數據庫初始化dal
            IDal iDal = IDal.InstanceSqlServer(connKey01);

            //取得數據
            DataTable dt = iDal.QuerySqlWithParameters(sql, lps) != null ? iDal.QuerySqlWithParameters(sql, lps).Tables[0] : null;
            //取得數據總數
            int totalRows = dt != null ? dt.Rows.Count : 0;
            //數據分頁:通用化,避免數據庫差異帶來分頁問題
            return new JsonData() { total = totalRows, rows = DataTableHelper.GetPagedTable(dt, pageIndex, pageSize) };
        }

        public DataTable GetAllLog()
        {
            LogHelper.WriteSqlDbLogForWarn("用户[" + UserInfo.EmpNo + "]" + UserInfo.EUserName + "下载了网站日志!");
            string sql = "SELECT DATETIME,THREAD,LOG_LEVEL,LOGGER,MESSAGE FROM F_LOG ";
            //為sqlserver數據庫初始化dal
            IDal iDal = IDal.InstanceSqlServer(connKey01);

            //取得數據
            return iDal.QuerySql(sql) != null ? iDal.QuerySql(sql).Tables[0] : null;
        }
    }
}
