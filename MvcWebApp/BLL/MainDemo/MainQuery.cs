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

namespace BLL.MainDemo
{
    /// <summary>
    /// 查詢頁邏輯
    /// </summary>
    public class MainQuery
    {
        /// <summary>
        /// webconfig中的連接字符串key
        /// </summary>
        string connKey01 = "ConnectionStringOracle";

        private int pageSize;
        private int pageIndex;
        public MainQuery()
        {
            //初始化參數
            pageSize = 10;
            pageIndex = 1;
        }

        /// <summary>
        /// 查詢方法
        /// </summary>
        /// <returns></returns>
        public JsonData Query(FormCollection collection, string _pageIndex, string _pageSize)
        {
            //string sql = "SELECT * FROM mainpagetestdata where 1=1 or Test1=:Test1 and Test2=:Test2";
            //List<object> lps = new List<object>();
            //OracleParameter spTest1 = new OracleParameter(":Test1", collection["text1"]);
            //OracleParameter spTest2 = new OracleParameter(":Test2", collection["text2"]);
            //lps.Add(spTest1);
            //lps.Add(spTest2);

            //取得分頁信息
            int.TryParse(_pageIndex, out pageIndex);
            int.TryParse(_pageSize, out pageSize);

            //防止分頁異常
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageIndex = pageIndex < 1 ? 1 : pageIndex;

            ////為sqlserver數據庫初始化dal
            //IDal iDal = IDal.InstanceOracle(connKey01);

            ////取得數據
            //DataTable dt = iDal.QuerySqlWithParameters(sql, lps) != null ? iDal.QuerySqlWithParameters(sql, lps).Tables[0] : null;
            ////取得數據總數
            //int totalRows = dt != null ? dt.Rows.Count : 0;
            ////數據分頁:通用化,避免數據庫差異帶來分頁問題
            //return new JsonData() { total = totalRows, rows = DataTableHelper.GetPagedTable(dt, pageIndex, pageSize) };

            DataTable dt = GetMainQueryTestData();
            return new JsonData() { total = dt.Rows.Count, rows = DataTableHelper.GetPagedTable(dt, pageIndex, pageSize) };
        }
        public MyJsonResult Delete(params string[] keys)
        {
            return new MyJsonResult() { code = 0, msg = "delete ok!" };
        }

        public DataTable GetHeader4Edit(string keys)
        {
            return GetMainQueryTestData();
        }

        private DataTable GetMainQueryTestData()
        {
            DataTable dtTmp = new DataTable();

            dtTmp.Columns.Add("KEYS");
            dtTmp.Columns.Add("TEST1");
            dtTmp.Columns.Add("TEST2");
            dtTmp.Columns.Add("TEST3");
            dtTmp.Columns.Add("TEST4");
            dtTmp.Columns.Add("TEST5");
            dtTmp.Columns.Add("TEST6");

            DataRow dr = dtTmp.NewRow();
            for (int i = 0; i < 45; i++)
            {
                dr["KEYS"] = "Pass1" + i;
                dr["TEST1"] = "Pass1" + i;
                dr["TEST2"] = "Pass2" + i;
                dr["TEST3"] = "Pass3" + i;
                dr["TEST4"] = "Pass4" + i;
                dr["TEST5"] = "Pass5" + i;
                dr["TEST6"] = "Pass6" + i;
                dtTmp.Rows.Add(dr);
                dr = dtTmp.NewRow();
            }
            return dtTmp;
        }
    }
}
