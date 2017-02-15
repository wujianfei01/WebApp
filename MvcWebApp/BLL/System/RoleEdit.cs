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
    public class RoleEdit
    {
        /// <summary>
        /// webconfig中的連接字符串key
        /// </summary>
        string connKey01 = "ConnectionStringOracle";


        /// <summary>
        /// 檢查是否存在記錄
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        private bool CheckIsExist(FormCollection collection)
        {
            //為sqlserver數據庫初始化dal
            IDal iDal = IDal.InstanceOracle(connKey01);
            DataSet ds = null;

            string sql = "SELECT 1 FROM F_ROLES WHERE 1=1 ";
            if (!string.IsNullOrEmpty(collection["KEYS"]))
            {
                sql += "AND ROLE_ID='{0}'";
                ds = iDal.QuerySql(string.Format(sql, collection["KEYS"]));
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
            //fc["POWER"]
            bool isExists = CheckIsExist(collection);
            int _effRows = 0;
            int _code = 0;
            string _msg = string.Empty;
            string sqlInsert = "INSERT INTO F_ROLES(ROLE_ID,ROLE_NAME,ROLE_DESCRIPTION,ENABLE_FLAG,CREATION_DATE,CREATED_BY,LAST_UPDATE_TIME,LAST_UPDATE_BY) VALUES" +
                               "((SELECT COUNT(1) FROM F_ROLES)+1,:ROLE_NAME,:ROLE_DESCRIPTION,:ENABLE_FLAG,SYSDATE,:CREATED_BY,NULL,NULL)";
            string sqlUpdate = "UPDATE F_ROLES SET ROLE_NAME=:ROLE_NAME,ROLE_DESCRIPTION=:ROLE_DESCRIPTION,ENABLE_FLAG=:ENABLE_FLAG,LAST_UPDATE_BY = :LAST_UPDATE_BY,LAST_UPDATE_TIME = SYSDATE WHERE ROLE_ID = :ROLE_ID";
            string sql = isExists ? sqlUpdate : sqlInsert;
            List<object> lps = new List<object>();
            OracleParameter spROLEID = new OracleParameter(":ROLE_ID", string.IsNullOrEmpty(collection["KEYS"]) ? null : collection["KEYS"]);

            OracleParameter spROLENAME = new OracleParameter(":ROLE_NAME", string.IsNullOrEmpty(collection["ROLE_NAME"]) ? null : collection["ROLE_NAME"]);
            OracleParameter spROLEDESCRIPTION = new OracleParameter(":ROLE_DESCRIPTION", string.IsNullOrEmpty(collection["ROLE_DESCRIPTION"]) ? null : collection["ROLE_DESCRIPTION"]);
            OracleParameter spENABLEFLAG = new OracleParameter(":ENABLE_FLAG", string.IsNullOrEmpty(collection["ENABLE_FLAG"]) ? null : collection["ENABLE_FLAG"]);

            OracleParameter spLASTUPDATEBY = new OracleParameter(":LAST_UPDATE_BY", string.Format("{0}({1})", UserInfo.EUserName, UserInfo.EmpNo));
            OracleParameter spCREATEDBY = new OracleParameter(":CREATED_BY", string.Format("{0}({1})",UserInfo.EUserName,UserInfo.EmpNo));
            lps.Add(spROLENAME);
            lps.Add(spROLEDESCRIPTION);
            lps.Add(spENABLEFLAG);

            if (isExists)
            {
                lps.Add(spROLEID);
                lps.Add(spLASTUPDATEBY);
            }
            else
                lps.Add(spCREATEDBY);


            //為sqlserver數據庫初始化dal
            IDal iDal = IDal.InstanceOracle(connKey01);

            //取得影响行数
            try
            {
                if (!string.IsNullOrEmpty(collection["KEYS"]))
                    DoRights(collection["KEYS"], collection["POWER"].Split(','));
                _effRows = iDal.OptSqlWithParameters(sql, lps);
                _code = 0;
                _msg = "Success:影響" + _effRows + "行";
            }
            catch (Exception ex)
            {
                _code = 1;
                _msg = "Fail:" + ex.ToString();//此處可以定義成你自己的報錯信息
            }
            LogHelper.WriteSqlDbLog("用户[" + UserInfo.EmpNo + "]" + UserInfo.EUserName + "角色编辑操作:" + _msg + "!");
            return new MyJsonResult() { code = _code, msg = _msg };
        }

        /// <summary>
        /// 同步權限
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="keys"></param>
        private void DoRights(string roleid, string[] keys)
        {
            IDal iDal = IDal.InstanceOracle(connKey01);
            iDal.OptSql(string.Format("DELETE F_RIGHT WHERE ROLE_ID='{0}'", string.IsNullOrEmpty(roleid) ? string.Empty : roleid));
            if (!string.IsNullOrEmpty(keys[0]))
            {
                foreach (string t in keys)
                {
                    iDal.OptSql(string.Format("INSERT INTO F_RIGHT(RIGHT_ID,ROLE_ID,NODE_ID,LAST_UPDATE_DATE,LAST_UPDATE_BY) VALUES('{0}',{1},'{2}',SYSDATE,'{3}')", Guid.NewGuid().ToString("N"), roleid, t, UserInfo.EUserName + "(" + UserInfo.EmpNo + ")"));
                }
            }
            LogHelper.WriteSqlDbLog("用户[" + UserInfo.EmpNo + "]" + UserInfo.EUserName + "角色编辑级联权限操作!");
        }

        /// <summary>
        /// 刪除菜單
        /// </summary>
        /// <param name="keys">物理主鍵</param>
        /// <returns></returns>
        public MyJsonResult Delete(params string[] keys)
        {
            int _effRows = 0;
            int _code = 0;
            string _msg = string.Empty;
            //為sqlserver數據庫初始化dal
            IDal iDal = IDal.InstanceOracle(connKey01);

            string sqlDelete = "DELETE F_ROLES WHERE ROLE_ID=:ROLE_ID"; //子菜單
            List<object> lps = new List<object>();
            OracleParameter spID = new OracleParameter(":ROLE_ID", string.IsNullOrEmpty(keys[0]) ? null : keys[0]);

            lps.Add(spID);

            //取得影响行数
            try
            {
                _effRows = iDal.OptSqlWithParameters(sqlDelete, lps);
                iDal.OptSql(string.Format("DELETE F_RIGHT WHERE ROLE_ID={0}", string.IsNullOrEmpty(keys[0]) ? string.Empty : keys[0]));//級聯操作:刪除role則相關權限也刪除
                _code = 0;
                _msg = "Success:影響" + _effRows + "行";
            }
            catch (Exception ex)
            {
                _code = 1;
                _msg = "Fail:" + ex.ToString();//此處可以定義成你自己的報錯信息
            }
            LogHelper.WriteSqlDbLog("用户[" + UserInfo.EmpNo + "]" + UserInfo.EUserName + "删除角色操作(删除关联权限)!");
            return new MyJsonResult() { code = _code, msg = _msg };
        }


    }

    public class RoleEditForSQLServer
    {
        /// <summary>
        /// webconfig中的連接字符串key
        /// </summary>
        string connKey01 = "ConnectionStringSqlServer";


        /// <summary>
        /// 檢查是否存在記錄
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        private bool CheckIsExist(FormCollection collection)
        {
            //為sqlserver數據庫初始化dal
            IDal iDal = IDal.InstanceSqlServer(connKey01);
            DataSet ds = null;

            string sql = "SELECT 1 FROM F_ROLES WHERE 1=1 ";
            if (!string.IsNullOrEmpty(collection["KEYS"]))
            {
                sql += "AND ROLE_ID='{0}'";
                ds = iDal.QuerySql(string.Format(sql, collection["KEYS"]));
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
            //fc["POWER"]
            bool isExists = CheckIsExist(collection);
            int _effRows = 0;
            int _code = 0;
            string _msg = string.Empty;
            string sqlInsert = "INSERT INTO F_ROLES(ROLE_ID,ROLE_NAME,ROLE_DESCRIPTION,ENABLE_FLAG,CREATION_DATE,CREATED_BY,LAST_UPDATE_TIME,LAST_UPDATE_BY) VALUES" +
                               "((SELECT COUNT(1) FROM F_ROLES)+1,@ROLE_NAME,@ROLE_DESCRIPTION,@ENABLE_FLAG,GETDATE(),@CREATED_BY,NULL,NULL)";
            string sqlUpdate = "UPDATE F_ROLES SET ROLE_NAME=@ROLE_NAME,ROLE_DESCRIPTION=@ROLE_DESCRIPTION,ENABLE_FLAG=@ENABLE_FLAG,LAST_UPDATE_BY = @LAST_UPDATE_BY,LAST_UPDATE_TIME = GETDATE() WHERE ROLE_ID = @ROLE_ID";
            string sql = isExists ? sqlUpdate : sqlInsert;
            List<object> lps = new List<object>();
            SqlParameter spROLEID = new SqlParameter("@ROLE_ID", string.IsNullOrEmpty(collection["KEYS"]) ? null : collection["KEYS"]);

            SqlParameter spROLENAME = new SqlParameter("@ROLE_NAME", string.IsNullOrEmpty(collection["ROLE_NAME"]) ? null : collection["ROLE_NAME"]);
            SqlParameter spROLEDESCRIPTION = new SqlParameter("@ROLE_DESCRIPTION", string.IsNullOrEmpty(collection["ROLE_DESCRIPTION"]) ? null : collection["ROLE_DESCRIPTION"]);
            SqlParameter spENABLEFLAG = new SqlParameter("@ENABLE_FLAG", string.IsNullOrEmpty(collection["ENABLE_FLAG"]) ? null : collection["ENABLE_FLAG"]);

            SqlParameter spLASTUPDATEBY = new SqlParameter("@LAST_UPDATE_BY", string.Format("{0}({1})", UserInfo.EUserName, UserInfo.EmpNo));
            SqlParameter spCREATEDBY = new SqlParameter("@CREATED_BY", string.Format("{0}({1})", UserInfo.EUserName, UserInfo.EmpNo));
            lps.Add(spROLENAME);
            lps.Add(spROLEDESCRIPTION);
            lps.Add(spENABLEFLAG);

            if (isExists)
            {
                lps.Add(spROLEID);
                lps.Add(spLASTUPDATEBY);
            }
            else
                lps.Add(spCREATEDBY);


            //為sqlserver數據庫初始化dal
            IDal iDal = IDal.InstanceSqlServer(connKey01);

            //取得影响行数
            try
            {
                if (!string.IsNullOrEmpty(collection["KEYS"]))
                    DoRights(collection["KEYS"], collection["POWER"].Split(','));
                _effRows = iDal.OptSqlWithParameters(sql, lps);
                _code = 0;
                _msg = "Success:影響" + _effRows + "行";
            }
            catch (Exception ex)
            {
                _code = 1;
                _msg = "Fail:" + ex.ToString();//此處可以定義成你自己的報錯信息
            }
            LogHelper.WriteSqlDbLog("用户[" + UserInfo.EmpNo + "]" + UserInfo.EUserName + "角色编辑操作:" + _msg + "!");
            return new MyJsonResult() { code = _code, msg = _msg };
        }

        /// <summary>
        /// 同步權限
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="keys"></param>
        private void DoRights(string roleid, string[] keys)
        {
            IDal iDal = IDal.InstanceSqlServer(connKey01);
            iDal.OptSql(string.Format("DELETE F_RIGHT WHERE ROLE_ID='{0}'", string.IsNullOrEmpty(roleid) ? string.Empty : roleid));
            if (!string.IsNullOrEmpty(keys[0]))
            {
                foreach (string t in keys)
                {
                    iDal.OptSql(string.Format("INSERT INTO F_RIGHT(RIGHT_ID,ROLE_ID,NODE_ID,LAST_UPDATE_DATE,LAST_UPDATE_BY) VALUES('{0}',{1},'{2}',GETDATE(),'{3}')", Guid.NewGuid().ToString("N"), roleid, t, UserInfo.EUserName + "(" + UserInfo.EmpNo + ")"));
                }
            }
            LogHelper.WriteSqlDbLog("用户[" + UserInfo.EmpNo + "]" + UserInfo.EUserName + "角色编辑级联权限操作!");
        }

        /// <summary>
        /// 刪除菜單
        /// </summary>
        /// <param name="keys">物理主鍵</param>
        /// <returns></returns>
        public MyJsonResult Delete(params string[] keys)
        {
            int _effRows = 0;
            int _code = 0;
            string _msg = string.Empty;
            //為sqlserver數據庫初始化dal
            IDal iDal = IDal.InstanceSqlServer(connKey01);

            string sqlDelete = "DELETE F_ROLES WHERE ROLE_ID=@ROLE_ID"; //子菜單
            List<object> lps = new List<object>();
            SqlParameter spID = new SqlParameter("@ROLE_ID", string.IsNullOrEmpty(keys[0]) ? null : keys[0]);

            lps.Add(spID);

            //取得影响行数
            try
            {
                _effRows = iDal.OptSqlWithParameters(sqlDelete, lps);
                iDal.OptSql(string.Format("DELETE F_RIGHT WHERE ROLE_ID={0}", string.IsNullOrEmpty(keys[0]) ? string.Empty : keys[0]));//級聯操作:刪除role則相關權限也刪除
                _code = 0;
                _msg = "Success:影響" + _effRows + "行";
            }
            catch (Exception ex)
            {
                _code = 1;
                _msg = "Fail:" + ex.ToString();//此處可以定義成你自己的報錯信息
            }
            LogHelper.WriteSqlDbLog("用户[" + UserInfo.EmpNo + "]" + UserInfo.EUserName + "删除角色操作(删除关联权限)!");
            return new MyJsonResult() { code = _code, msg = _msg };
        }


    }
}
