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
    public class RoleQuery
    {
        /// <summary>
        /// webconfig中的連接字符串key
        /// </summary>
        string connKey01 = "ConnectionStringOracle";

        private int pageSize;
        private int pageIndex;
        public RoleQuery()
        {
            pageSize = 10;
            pageIndex = 1;
        }
        public string GetPowerForInit(string key)
        {
            string ret = string.Empty;
            IDal iDal = IDal.InstanceOracle(connKey01);
            DataTable dt = iDal.QuerySql(string.Format("SELECT RIGHT_ID FROM F_RIGHT WHERE ROLE_ID='{0}'", key)).Tables[0];
            if (null != dt && dt.Rows.Count > 0)
            {
                int count=dt.Rows.Count;
                for (int i = 0; i < count; i++)
                {
                    ret += dt.Rows[i][0].ToString() + ",";
                }      
            }

            return ret.Trim(',');
        }

        public JsonData Query(FormCollection collection, string _pageIndex, string _pageSize)
        {

            string sql = "SELECT ROLE_ID KEYS,ROLE_ID,ROLE_NAME,ROLE_DESCRIPTION,ENABLE_FLAG,CREATION_DATE,CREATED_BY,LAST_UPDATE_TIME,LAST_UPDATE_BY FROM F_ROLES WHERE 1 = 1 ";
            List<object> lps = new List<object>();
            if (!string.IsNullOrEmpty(collection["roleid"]))
            {
                sql += "AND ROLE_ID LIKE :ROLE_ID";
                OracleParameter sproleid = new OracleParameter(":ROLE_ID", "%" + collection["roleid"] + "%");
                lps.Add(sproleid);
            }
            if (!string.IsNullOrEmpty(collection["rolename"]))
            {
                sql += "AND ROLE_NAME LIKE :ROLE_NAME";
                OracleParameter sprolename = new OracleParameter(":ROLE_NAME", "%" + collection["rolename"] + "%");
                lps.Add(sprolename);
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

    public class RoleQueryForSQLServer
    {
        /// <summary>
        /// webconfig中的連接字符串key
        /// </summary>
        string connKey01 = "ConnectionStringSqlServer";

        private int pageSize;
        private int pageIndex;
        public RoleQueryForSQLServer()
        {
            pageSize = 10;
            pageIndex = 1;
        }
        public string GetPowerForInit(string key)
        {
            string ret = string.Empty;
            IDal iDal = IDal.InstanceSqlServer(connKey01);
            DataTable dt = iDal.QuerySql(string.Format("SELECT RIGHT_ID FROM F_RIGHT WHERE ROLE_ID='{0}'", key)).Tables[0];
            if (null != dt && dt.Rows.Count > 0)
            {
                int count = dt.Rows.Count;
                for (int i = 0; i < count; i++)
                {
                    ret += dt.Rows[i][0].ToString() + ",";
                }
            }

            return ret.Trim(',');
        }

        public JsonData Query(FormCollection collection, string _pageIndex, string _pageSize)
        {

            string sql = "SELECT ROLE_ID KEYS,ROLE_ID,ROLE_NAME,ROLE_DESCRIPTION,ENABLE_FLAG,CREATION_DATE,CREATED_BY,LAST_UPDATE_TIME,LAST_UPDATE_BY FROM F_ROLES WHERE 1 = 1 ";
            List<object> lps = new List<object>();
            if (!string.IsNullOrEmpty(collection["roleid"]))
            {
                sql += "AND ROLE_ID LIKE @ROLE_ID";
                SqlParameter sproleid = new SqlParameter("@ROLE_ID", "%" + collection["roleid"] + "%");
                lps.Add(sproleid);
            }
            if (!string.IsNullOrEmpty(collection["rolename"]))
            {
                sql += "AND ROLE_NAME LIKE @ROLE_NAME";
                SqlParameter sprolename = new SqlParameter("@ROLE_NAME", "%" + collection["rolename"] + "%");
                lps.Add(sprolename);
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
