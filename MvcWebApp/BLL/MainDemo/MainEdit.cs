using DAL.Interface;
using MODEL;
using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace BLL.MainDemo
{
    /// <summary>
    /// 編輯頁邏輯
    /// </summary>
    public class MainEdit
    {
        /// <summary>
        /// webconfig中的連接字符串key
        /// </summary>
        string connKey01 = "ConnectionStringOracle";

        public MyJsonResult Edit(FormCollection collection)
        {
            int _effRows = 0;
            int _code = 0;
            string _msg = string.Empty;
            string sql = "update MainPageTestData set test6='數據被更新了' where 1=1 and Test1=:Test1 and Test2=:Test2";
            List<object> lps = new List<object>();
            OracleParameter spTest1 = new OracleParameter(":Test1", collection["text1"]);
            OracleParameter spTest2 = new OracleParameter(":Test2", collection["text2"]);
            lps.Add(spTest1);
            lps.Add(spTest2);

            //為sqlserver數據庫初始化dal
            IDal iDal = IDal.InstanceOracle(connKey01);

            //取得影响行数
            try
            {
                _effRows = iDal.OptSqlWithParameters(sql, lps);
                _code = 0;
                _msg = "Success:影響" + _effRows + "行";
            }
            catch (Exception ex)
            {
                _code = 1;
                _msg = "Fail:" + ex.ToString();//此處可以定義成你自己的報錯信息
            }

            return new MyJsonResult() { code = _code, msg = _msg };
        }
    }
}
