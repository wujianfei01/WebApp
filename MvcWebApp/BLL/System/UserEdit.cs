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

namespace BLL.MainDemo
{
    /// <summary>
    /// 編輯頁邏輯
    /// </summary>
    public class UserEdit
    {
        /// <summary>
        /// webconfig中的連接字符串key
        /// </summary>
        string connKey01 = "ConnectionStringOracle";

        private bool CheckIsExist(FormCollection collection)
        {
            //為sqlserver數據庫初始化dal
            IDal iDal = IDal.InstanceOracle(connKey01);
            DataSet ds = null;

            string sql = "SELECT 1 FROM F_USERS WHERE 1=1 ";
            if (!string.IsNullOrEmpty(collection["USER_ID"]))
            {
                sql += "AND USER_ID='{0}'";
                ds = iDal.QuerySql(string.Format(sql, collection["USER_ID"]));
            }
            else
                return false;

            if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count.Equals(0))
                return false;
            else
                return true;
        }


        /// <summary>
        /// 保存或者新增公用方法
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public MyJsonResult SaveOrUpdate(FormCollection collection)
        {
            int _effRows = 0;
            int _code = 0;
            string _msg = string.Empty;
            bool upOrIns = CheckIsExist(collection);
            string sqlInsert = "INSERT INTO F_USERS(USER_ID,ROLE_ID,ENABLE_FLAG,CREATION_DATE,CREATED_BY,E_USER_NAME,USER_NAME,DEPT_NO,EMAIL) VALUES(:USER_ID,:ROLE_ID,:ENABLE_FLAG,SYSDATE,:CREATED_BY,:E_USER_NAME,:USER_NAME,:DEPT_NO,:EMAIL)";
            string sqlUpdate = "UPDATE F_USERS SET ROLE_ID=:ROLE_ID,ENABLE_FLAG=:ENABLE_FLAG,LAST_UPDATE_TIME=SYSDATE,LAST_UPDATE_BY=:CREATED_BY WHERE USER_ID=:USER_ID";
            string sql = upOrIns ? sqlUpdate : sqlInsert;
            List<object> lps = new List<object>();
            OracleParameter spUSER_ID = new OracleParameter(":USER_ID", collection["USER_ID"]);
            OracleParameter spROLE_ID = new OracleParameter(":ROLE_ID", collection["ROLE_ID"]);
            OracleParameter spENABLE_FLAG = new OracleParameter(":ENABLE_FLAG", collection["ENABLE_FLAG"]);
            OracleParameter spCREATED_BY = new OracleParameter(":CREATED_BY", UserInfo.EmpNo);

            OracleParameter spE_USER_NAME = new OracleParameter(":E_USER_NAME", collection["E_USER_NAME"]);
            OracleParameter spUSER_NAME = new OracleParameter(":USER_NAME", collection["USER_NAME"]);
            OracleParameter spDEPT_NO = new OracleParameter(":DEPT_NO", collection["DEPT_NO"]);
            OracleParameter spEMAIL = new OracleParameter(":EMAIL", collection["EMAIL"]);

            lps.Add(spUSER_ID);
            lps.Add(spROLE_ID);
            lps.Add(spENABLE_FLAG);
            lps.Add(spCREATED_BY);

            if (!upOrIns)
            {
                lps.Add(spE_USER_NAME);
                lps.Add(spUSER_NAME);
                lps.Add(spDEPT_NO);
                lps.Add(spEMAIL);
            }

            IDal iDal = IDal.InstanceOracle(connKey01);

            try
            {
                _effRows = iDal.OptSqlWithParameters(sql, lps);
                _code = 0;
                _msg = "Success:影響" + _effRows + "行";
            }
            catch (Exception ex)
            {
                _code = 1;
                _msg = "Edir User Fail:" + ex.ToString();
            }
            LogHelper.WriteSqlDbLog("用户[" + UserInfo.EmpNo + "]" + UserInfo.EUserName + "删除用户操作:" + _msg + "!");
            return new MyJsonResult() { code = _code, msg = _msg };
        }

        /// <summary>
        /// Format Error Json
        /// </summary>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public MyJsonResult CommonAlertMsg(int code = 1, string msg = "Error")
        {
            return new MyJsonResult() { code = code, msg = msg };
        }

        /// <summary>
        /// 刪除用戶
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public MyJsonResult Delete(params string[] keys)
        {
            int _effRows = 0;
            int _code = 0;
            string _msg = string.Empty;
            string sqlDelete = "DELETE F_USERS WHERE USER_ID=:USER_ID ";
            List<object> lps = new List<object>();
            OracleParameter spUSER_ID = new OracleParameter(":USER_ID", keys[0]);

            lps.Add(spUSER_ID);

            IDal iDal = IDal.InstanceOracle(connKey01);

            try
            {
                _effRows = iDal.OptSqlWithParameters(sqlDelete, lps);
                _code = 0;
                _msg = "Success:影響" + _effRows + "行";
            }
            catch (Exception ex)
            {
                _code = 1;
                _msg = "Delete User Fail:" + ex.ToString();
            }
            LogHelper.WriteSqlDbLog("用户[" + UserInfo.EmpNo + "]" + UserInfo.EUserName + "删除用户操作:" + _msg + "!");
            return new MyJsonResult() { code = _code, msg = _msg };
        }

        public List<JsonCombox> GetComboxDataForRoleId()
        {
            List<JsonCombox> ls = new List<JsonCombox>();
            string sql = "SELECT ROLE_ID AS ID,ROLE_ID||' | '||ROLE_NAME AS TEXT FROM F_ROLES ";

            //為sqlserver數據庫初始化dal
            IDal iDal = IDal.InstanceOracle(connKey01);
            DataSet ds = iDal.QuerySql(sql);
            if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count.Equals(0))
                return ls;
            else
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    JsonCombox jc = new JsonCombox();
                    jc.id = dr["id"].ToString();
                    jc.text = dr["text"].ToString();
                    ls.Add(jc);
                }

                return ls;
            }

        }

        public List<JsonCombox> GetComboxDataForEnableFlag()
        {
            List<JsonCombox> ls = new List<JsonCombox>();
            ls.Add(new JsonCombox() { id = "0", text = "启用" });
            ls.Add(new JsonCombox() { id = "1", text = "禁用" });
            return ls;
        }

    }

    public class UserEditForSQLServer
    {
        /// <summary>
        /// webconfig中的連接字符串key
        /// </summary>
        string connKey01 = "ConnectionStringSqlServer";

        private bool CheckIsExist(FormCollection collection)
        {
            IDal iDal = IDal.InstanceSqlServer(connKey01);
            DataSet ds = null;

            string sql = "SELECT 1 FROM F_USERS WHERE 1=1 ";
            if (!string.IsNullOrEmpty(collection["USER_ID"]))
            {
                sql += "AND USER_ID='{0}'";
                ds = iDal.QuerySql(string.Format(sql, collection["USER_ID"]));
            }
            else
                return false;

            if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count.Equals(0))
                return false;
            else
                return true;
        }

        /// <summary>
        /// 保存或者新增公用方法
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public MyJsonResult SaveOrUpdate(FormCollection collection)
        {
            int _effRows = 0;
            int _code = 0;
            string _msg = string.Empty;
            string sqlInsert = "INSERT INTO F_USERS(USER_ID,ROLE_ID,ENABLE_FLAG,CREATION_DATE,CREATED_BY,E_USER_NAME,USER_NAME,DEPT_NO,EMAIL) VALUES(@USER_ID,@ROLE_ID,@ENABLE_FLAG,GETDATE(),'FROMWBS','FROMWBS','FROMWBS','FROMWBS','FROMWBS')";
            string sqlUpdate = "UPDATE F_USERS SET ROLE_ID=@ROLE_ID,ENABLE_FLAG=@ENABLE_FLAG,LAST_UPDATE_TIME=GETDATE(),LAST_UPDATE_BY='FROMWBS' WHERE USER_ID=@USER_ID";
            string sql = CheckIsExist(collection) ? sqlUpdate : sqlInsert;
            List<object> lps = new List<object>();
            SqlParameter spUSER_ID = new SqlParameter("@USER_ID", collection["USER_ID"]);
            SqlParameter spROLE_ID = new SqlParameter("@ROLE_ID", collection["ROLE_ID"]);
            SqlParameter spENABLE_FLAG = new SqlParameter("@ENABLE_FLAG", collection["ENABLE_FLAG"]);

            lps.Add(spUSER_ID);
            lps.Add(spROLE_ID);
            lps.Add(spENABLE_FLAG);

            IDal iDal = IDal.InstanceSqlServer(connKey01);

            try
            {
                _effRows = iDal.OptSqlWithParameters(sql, lps);
                _code = 0;
                _msg = "Success:影響" + _effRows + "行";
            }
            catch (Exception ex)
            {
                _code = 1;
                _msg = "Edir User Fail:" + ex.ToString();
            }
            LogHelper.WriteSqlDbLog("用户[" + UserInfo.EmpNo + "]" + UserInfo.EUserName + "编辑用户操作:" + _msg + "!");
            return new MyJsonResult() { code = _code, msg = _msg };
        }

        /// <summary>
        /// 刪除用戶
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public MyJsonResult Delete(params string[] keys)
        {
            int _effRows = 0;
            int _code = 0;
            string _msg = string.Empty;
            string sqlDelete = "DELETE F_USERS WHERE USER_ID=@USER_ID ";
            List<object> lps = new List<object>();
            SqlParameter spUSER_ID = new SqlParameter("@USER_ID", keys[0]);

            lps.Add(spUSER_ID);

            IDal iDal = IDal.InstanceSqlServer(connKey01);

            try
            {
                _effRows = iDal.OptSqlWithParameters(sqlDelete, lps);
                _code = 0;
                _msg = "Success:影響" + _effRows + "行";
            }
            catch (Exception ex)
            {
                _code = 1;
                _msg = "Delete User Fail:" + ex.ToString();
            }
            LogHelper.WriteSqlDbLog("用戶[" + UserInfo.EmpNo + "]" + UserInfo.EUserName + "用戶刪除操作:" + _msg + "!");
            return new MyJsonResult() { code = _code, msg = _msg };
        }



        public List<JsonCombox> GetComboxDataForRoleId()
        {
            List<JsonCombox> ls = new List<JsonCombox>();
            string sql = "SELECT ROLE_ID AS ID,CONVERT(VARCHAR(10), ROLE_ID)+' | '+ROLE_NAME AS TEXT FROM F_ROLES ";

            IDal iDal = IDal.InstanceSqlServer(connKey01);
            DataSet ds = iDal.QuerySql(sql);
            if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count.Equals(0))
                return ls;
            else
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    JsonCombox jc = new JsonCombox();
                    jc.id = dr["id"].ToString();
                    jc.text = dr["text"].ToString();
                    ls.Add(jc);
                }

                return ls;
            }

        }
    }
}
