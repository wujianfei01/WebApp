using DAL.Interface;
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
    public class MenuQuery
    {
        /// <summary>
        /// webconfig中的連接字符串key
        /// </summary>
        string connKey01 = "ConnectionStringOracle";

        private int pageSize;
        private int pageIndex;
        public MenuQuery()
        {
            pageSize = 10;
            pageIndex = 1;
        }

        public JsonData Query(FormCollection collection, string _pageIndex, string _pageSize)
        {
            string sql = "SELECT FT.ID KEYS,FT.*,CASE WHEN FT.PARENTID='0' THEN '主' ELSE '子菜单' END AS MENUTYPE FROM F_TREE FT LEFT JOIN F_ROLES FR ON ROLE_ID=ROLEID WHERE 1 = 1 ";
            List<object> lps = new List<object>();
            if (!string.IsNullOrEmpty(collection["menuname"]))
            {
                sql += "AND UPPER(MENUNAME) LIKE :MENUNAME";
                OracleParameter spmenuname = new OracleParameter(":MENUNAME", "%" + collection["menuname"].ToUpper() + "%");
                lps.Add(spmenuname);
            }

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

    public class MenuQueryForSQLServer
    {
        /// <summary>
        /// webconfig中的連接字符串key
        /// </summary>
        string connKey01 = "ConnectionStringSqlServer";

        private int pageSize;
        private int pageIndex;
        public MenuQueryForSQLServer()
        {
            pageSize = 10;
            pageIndex = 1;
        }

        public JsonData Query(FormCollection collection, string _pageIndex, string _pageSize)
        {
            string sql = "SELECT FT.ID KEYS,FT.*,CASE WHEN FT.PARENTID='0' THEN '主' ELSE '子菜单' END AS MENUTYPE FROM F_TREE FT LEFT JOIN F_ROLES FR ON ROLE_ID=ROLEID WHERE 1 = 1 ";
            List<object> lps = new List<object>();
            if (!string.IsNullOrEmpty(collection["menuname"]))
            {
                sql += "AND UPPER(MENUNAME) LIKE @MENUNAME";
                SqlParameter spmenuname = new SqlParameter("@MENUNAME", "%" + collection["menuname"].ToUpper() + "%");
                lps.Add(spmenuname);
            }

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
    }
}
