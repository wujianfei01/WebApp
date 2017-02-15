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
    /// 系統編輯頁邏輯 For OracleDB
    /// </summary>
    public class MenuEdit
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

            string sql = "SELECT 1 FROM F_TREE WHERE 1=1 ";
            if (!string.IsNullOrEmpty(collection["ID"]))
            {
                sql += "AND ID='{0}'";
                ds = iDal.QuerySql(string.Format(sql, collection["ID"]));
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
            string guid = Guid.NewGuid().ToString("N");
            int _effRows = 0;
            int _code = 0;
            string _msg = string.Empty;
            string sqlInsert = "INSERT INTO F_TREE(ID,MENUNAME,PARENTID,STATE,URL,ENABLE) " +
                         "VALUES(:NEWID,:MENUNAME,:PARENTID,:STATE,:URL,:ENABLE)";
            string sqlUpdate = "UPDATE F_TREE SET MENUNAME=:MENUNAME,PARENTID=:PARENTID,STATE=:STATE,URL=:URL,ENABLE=:ENABLE WHERE ID=:ID";
            string sql = CheckIsExist(collection) ? sqlUpdate : sqlInsert;
            List<object> lps = new List<object>();
            OracleParameter spNEWID = new OracleParameter(":NEWID", guid);
            OracleParameter spID = new OracleParameter(":ID", string.IsNullOrEmpty(collection["ID"]) ? null : collection["ID"]);
            OracleParameter spMENUNAME = new OracleParameter(":MENUNAME", string.IsNullOrEmpty(collection["MENUNAME"]) ? null : collection["MENUNAME"]);
            OracleParameter spPARENTID = new OracleParameter(":PARENTID", string.IsNullOrEmpty(collection["PARENTID"]) ? null : (collection["PARENTID"].Equals("--主菜单--") ? "0" : collection["PARENTID"]));
            OracleParameter spSTATE = new OracleParameter(":STATE", string.IsNullOrEmpty(collection["STATE"]) ? null : collection["STATE"]);
            OracleParameter spURL = new OracleParameter(":URL", string.IsNullOrEmpty(collection["URL"]) ? null : collection["URL"]);
            OracleParameter spENABLE = new OracleParameter(":ENABLE", string.IsNullOrEmpty(collection["ENABLE"]) ? null : collection["ENABLE"]);
            
            lps.Add(spMENUNAME);
            lps.Add(spPARENTID);
            lps.Add(spSTATE);
            lps.Add(spURL);
            lps.Add(spENABLE);

            if (CheckIsExist(collection))
                lps.Add(spID);
            else
                lps.Add(spNEWID);

            //為sqlserver數據庫初始化dal
            IDal iDal = IDal.InstanceOracle(connKey01);

            //取得影响行数
            try
            {
                _effRows = iDal.OptSqlWithParameters(sql, lps);
                _code = 0;
                _msg = "Success:影响" + _effRows + "行";
            }
            catch (Exception ex)
            {
                _code = 1;
                _msg = "Fail:" + ex.ToString();//此處可以定義成你自己的報錯信息
            }
            LogHelper.WriteSqlDbLog("用户[" + UserInfo.EmpNo + "]" + UserInfo.EUserName + "菜单编辑操作:" + _msg + "!");
            return new MyJsonResult() { code = _code, msg = _msg };
        }

        /// <summary>
        /// 刪除菜單
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public MyJsonResult Delete(params string[] keys)
        {
            int _effRows = 0;
            int _code = 0;
            string _msg = string.Empty;
            //為sqlserver數據庫初始化dal
            IDal iDal = IDal.InstanceOracle(connKey01);
            string sql = string.Format("SELECT PARENTID FROM F_TREE WHERE ID='{0}' ", keys[0]);
            bool isMainMenu = iDal.QuerySql(sql).Tables[0].Rows[0][0].ToString().Equals("0") ? true : false;//所刪除的菜單是否為主菜單?主菜單:刪除主菜單以及下面的子菜單;子菜單:只刪除子菜單
            string sqlDelete = string.Empty;
            if (!isMainMenu)
                sqlDelete = "DELETE F_TREE WHERE ID=:ID"; //子菜單
            else
                sqlDelete = "DELETE F_TREE WHERE ID IN(SELECT ID FROM F_TREE WHERE PARENTID IN(:ID)) OR ID=:ID"; //主菜單
            List<object> lps = new List<object>();
            OracleParameter spID = new OracleParameter(":ID", string.IsNullOrEmpty(keys[0]) ? null : keys[0]);

            lps.Add(spID);

            //取得影响行数
            try
            {
                DelRights(keys[0]);
                _effRows = iDal.OptSqlWithParameters(sqlDelete, lps);
                _code = 0;
                _msg = "Success:影響" + _effRows + "行";
            }
            catch (Exception ex)
            {
                _code = 1;
                _msg = "Fail:" + ex.ToString();//此處可以定義成你自己的報錯信息
            }
            LogHelper.WriteSqlDbLog("用户[" + UserInfo.EmpNo + "]" + UserInfo.EUserName + "菜单删除操作:" + _msg + "!");
            return new MyJsonResult() { code = _code, msg = _msg };
        }

        /// <summary>
        /// parentid combox數據
        /// </summary>
        /// <returns></returns>
        public List<JsonCombox> GetComboxDataForParentId()
        {
            List<JsonCombox> ls = new List<JsonCombox>();
            string sql = "SELECT ID,MENUNAME AS TEXT FROM F_TREE WHERE PARENTID='0'";    

            //為sqlserver數據庫初始化dal
            IDal iDal = IDal.InstanceOracle(connKey01);
            DataSet ds = iDal.QuerySql(sql);
            if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count.Equals(0))
                return ls;
            else
            {
                ls.Add(new JsonCombox() { id = "0", text = "--主菜单--" });//为主菜单
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

        public List<JsonCombox> GetComboxDataForRoleId()
        {
            List<JsonCombox> ls = new List<JsonCombox>();
            string sql = "SELECT ROLE_ID AS ID,ROLE_DESCRIPTION AS TEXT FROM F_ROLES";

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



        /// <summary>
        /// 級聯操作:刪除rights表資料
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="nodeid"></param>
        private void DelRights(string treeKey)
        {
            string delsql = string.Format("DELETE F_RIGHT WHERE NODE_ID IN(SELECT ID FROM F_TREE WHERE ID='{0}' OR PARENTID='{0}')", treeKey);
            //為sqlserver數據庫初始化dal
            IDal iDal = IDal.InstanceOracle(connKey01);
            iDal.OptSql(delsql);
            LogHelper.WriteSqlDbLog("用户[" + UserInfo.EmpNo + "]" + UserInfo.EUserName + "菜单删除操作:级联删除了权限!");
        }


        public List<MyJsonTree> GetMenu(string key)
        {
            GetTreeMenu bll = new GetTreeMenu();
            string sql = string.IsNullOrEmpty(key) ? "SELECT FT.*,'false' CHECKED FROM F_TREE FT" : string.Format("SELECT FT.*,'true' CHECKED FROM F_TREE FT WHERE FT.ID IN(SELECT NODE_ID FROM F_RIGHT WHERE ROLE_ID = {0})" +
                           " UNION ALL " +
                        "SELECT FT.*,'false' CHECKED FROM F_TREE FT WHERE FT.ID NOT IN(SELECT NODE_ID FROM F_RIGHT WHERE ROLE_ID = {0})", key);

            //為sqlserver數據庫初始化dal
            IDal iDal = IDal.InstanceOracle(connKey01);
            DataSet ds = iDal.QuerySql(sql);
            if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count.Equals(0))
                return new List<MyJsonTree>();
            else
                return bll.initMyTree(ds.Tables[0]);
        }

        public MyJsonResult SetMenu(string roleid,string rightsid)
        {
            int _effRows = 0;
            int _code = 0;
            string _msg = string.Empty;
            //為sqlserver數據庫初始化dal
            IDal iDal = IDal.InstanceOracle(connKey01);
            string sql = string.Format("SELECT PARENTID FROM F_TREE WHERE ID='{0}' ", "");
            bool isMainMenu = iDal.QuerySql(sql).Tables[0].Rows[0][0].ToString().Equals("0") ? true : false;//所刪除的菜單是否為主菜單?主菜單:刪除主菜單以及下面的子菜單;子菜單:只刪除子菜單
            string sqlDelete = string.Empty;
            if (!isMainMenu)
                sqlDelete = "DELETE F_TREE WHERE ID=:ID"; //子菜單
            else
                sqlDelete = "DELETE F_TREE WHERE ID IN(SELECT ID FROM F_TREE WHERE PARENTID IN(:ID)) OR ID=:ID"; //主菜單
            List<object> lps = new List<object>();
            OracleParameter spID = new OracleParameter(":ID", string.IsNullOrEmpty("") ? null : "");

            lps.Add(spID);

            //取得影响行数
            try
            {
                DelRights("");
                _effRows = iDal.OptSqlWithParameters(sqlDelete, lps);
                _code = 0;
                _msg = "Success:影響" + _effRows + "行";
            }
            catch (Exception ex)
            {
                _code = 1;
                _msg = "Fail:" + ex.ToString();//此處可以定義成你自己的報錯信息
            }
            LogHelper.WriteSqlDbLog("用户[" + UserInfo.EmpNo + "]" + UserInfo.EUserName + "菜单权限编辑操作:" + _msg + "!");
            return new MyJsonResult() { code = _code, msg = _msg };
        }
    }

    /// <summary>
    /// 系統編輯頁邏輯 For SQLServer
    /// </summary>
    public class MenuEditForSQLServer
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

            string sql = "SELECT 1 FROM F_TREE WHERE 1=1 ";
            if (!string.IsNullOrEmpty(collection["ID"]))
            {
                sql += "AND ID='{0}'";
                ds = iDal.QuerySql(string.Format(sql, collection["ID"]));
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
            string guid = Guid.NewGuid().ToString("N");
            int _effRows = 0;
            int _code = 0;
            string _msg = string.Empty;
            string sqlInsert = "INSERT INTO F_TREE(ID,MENUNAME,PARENTID,STATE,URL,ENABLE) " +
                         "VALUES(@NEWID,@MENUNAME,@PARENTID,@STATE,@URL,@ENABLE)";
            string sqlUpdate = "UPDATE F_TREE SET MENUNAME=@MENUNAME,PARENTID=@PARENTID,STATE=@STATE,URL=@URL,ENABLE=@ENABLE WHERE ID=@ID";
            string sql = CheckIsExist(collection) ? sqlUpdate : sqlInsert;
            List<object> lps = new List<object>();
            SqlParameter spNEWID = new SqlParameter("@NEWID", guid);
            SqlParameter spID = new SqlParameter("@ID", string.IsNullOrEmpty(collection["ID"]) ? null : collection["ID"]);
            SqlParameter spMENUNAME = new SqlParameter("@MENUNAME", string.IsNullOrEmpty(collection["MENUNAME"]) ? null : collection["MENUNAME"]);
            SqlParameter spPARENTID = new SqlParameter("@PARENTID", string.IsNullOrEmpty(collection["PARENTID"]) ? null : (collection["PARENTID"].Equals("--主菜单--") ? "0" : collection["PARENTID"]));
            SqlParameter spSTATE = new SqlParameter("@STATE", string.IsNullOrEmpty(collection["STATE"]) ? null : collection["STATE"]);
            SqlParameter spURL = new SqlParameter("@URL", string.IsNullOrEmpty(collection["URL"]) ? null : collection["URL"]);
            SqlParameter spENABLE = new SqlParameter("@ENABLE", string.IsNullOrEmpty(collection["ENABLE"]) ? null : collection["ENABLE"]);

            lps.Add(spMENUNAME);
            lps.Add(spPARENTID);
            lps.Add(spSTATE);
            lps.Add(spURL);
            lps.Add(spENABLE);

            if (CheckIsExist(collection))
                lps.Add(spID);
            else
                lps.Add(spNEWID);

            //為sqlserver數據庫初始化dal
            IDal iDal = IDal.InstanceSqlServer(connKey01);

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
            LogHelper.WriteSqlDbLog("用户[" + UserInfo.EmpNo + "]" + UserInfo.EUserName + "菜单编辑操作:" + _msg + "!");
            return new MyJsonResult() { code = _code, msg = _msg };
        }

        /// <summary>
        /// 刪除菜單
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public MyJsonResult Delete(params string[] keys)
        {
            int _effRows = 0;
            int _code = 0;
            string _msg = string.Empty;
            //為sqlserver數據庫初始化dal
            IDal iDal = IDal.InstanceSqlServer(connKey01);
            string sql = string.Format("SELECT PARENTID FROM F_TREE WHERE ID='{0}' ", keys[0]);
            bool isMainMenu = iDal.QuerySql(sql).Tables[0].Rows[0][0].ToString().Equals("0") ? true : false;//所刪除的菜單是否為主菜單?主菜單:刪除主菜單以及下面的子菜單;子菜單:只刪除子菜單
            string sqlDelete = string.Empty;
            if (!isMainMenu)
                sqlDelete = "DELETE F_TREE WHERE ID=@ID"; //子菜單
            else
                sqlDelete = "DELETE F_TREE WHERE ID IN(SELECT ID FROM F_TREE WHERE PARENTID IN(@ID)) OR ID=@ID"; //主菜單
            List<object> lps = new List<object>();
            SqlParameter spID = new SqlParameter("@ID", string.IsNullOrEmpty(keys[0]) ? null : keys[0]);

            lps.Add(spID);

            //取得影响行数
            try
            {
                DelRights(keys[0]);
                _effRows = iDal.OptSqlWithParameters(sqlDelete, lps);
                _code = 0;
                _msg = "Success:影響" + _effRows + "行";
            }
            catch (Exception ex)
            {
                _code = 1;
                _msg = "Fail:" + ex.ToString();//此處可以定義成你自己的報錯信息
            }
            LogHelper.WriteSqlDbLog("用户[" + UserInfo.EmpNo + "]" + UserInfo.EUserName + "菜单删除操作:" + _msg + "!");
            return new MyJsonResult() { code = _code, msg = _msg };
        }

        /// <summary>
        /// parentid combox數據
        /// </summary>
        /// <returns></returns>
        public List<JsonCombox> GetComboxDataForParentId()
        {
            List<JsonCombox> ls = new List<JsonCombox>();
            string sql = "SELECT ID,MENUNAME AS TEXT FROM F_TREE WHERE PARENTID='0'";

            //為sqlserver數據庫初始化dal
            IDal iDal = IDal.InstanceSqlServer(connKey01);
            DataSet ds = iDal.QuerySql(sql);
            if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count.Equals(0))
                return ls;
            else
            {
                ls.Add(new JsonCombox() { id = "0", text = "--主菜单--" });//為主菜單
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

        public List<JsonCombox> GetComboxDataForRoleId()
        {
            List<JsonCombox> ls = new List<JsonCombox>();
            string sql = "SELECT ROLE_ID AS ID,ROLE_DESCRIPTION AS TEXT FROM F_ROLES";

            //為sqlserver數據庫初始化dal
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



        /// <summary>
        /// 級聯操作:刪除rights表資料
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="nodeid"></param>
        private void DelRights(string treeKey)
        {
            string delsql = string.Format("DELETE F_RIGHT WHERE NODE_ID IN(SELECT ID FROM F_TREE WHERE ID='{0}' OR PARENTID='{0}')", treeKey);
            //為sqlserver數據庫初始化dal
            IDal iDal = IDal.InstanceSqlServer(connKey01);
            iDal.OptSql(delsql);
            LogHelper.WriteSqlDbLog("用户[" + UserInfo.EmpNo + "]" + UserInfo.EUserName + "菜单删除操作:级联删除了权限!");
        }


        public List<MyJsonTree> GetMenu(string key)
        {
            GetTreeMenu bll = new GetTreeMenu();
            string sql = string.IsNullOrEmpty(key) ? "SELECT FT.*,'false' CHECKED FROM F_TREE FT" : string.Format("SELECT FT.*,'true' CHECKED FROM F_TREE FT WHERE FT.ID IN(SELECT NODE_ID FROM F_RIGHT WHERE ROLE_ID = {0})" +
                           " UNION ALL " +
                        "SELECT FT.*,'false' CHECKED FROM F_TREE FT WHERE FT.ID NOT IN(SELECT NODE_ID FROM F_RIGHT WHERE ROLE_ID = {0})", key);

            //為sqlserver數據庫初始化dal
            IDal iDal = IDal.InstanceSqlServer(connKey01);
            DataSet ds = iDal.QuerySql(sql);
            if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count.Equals(0))
                return new List<MyJsonTree>();
            else
                return bll.initMyTree(ds.Tables[0]);
        }

        public MyJsonResult SetMenu(string roleid, string rightsid)
        {
            int _effRows = 0;
            int _code = 0;
            string _msg = string.Empty;
            //為sqlserver數據庫初始化dal
            IDal iDal = IDal.InstanceSqlServer(connKey01);
            string sql = string.Format("SELECT PARENTID FROM F_TREE WHERE ID='{0}' ", "");
            bool isMainMenu = iDal.QuerySql(sql).Tables[0].Rows[0][0].ToString().Equals("0") ? true : false;//所刪除的菜單是否為主菜單?主菜單:刪除主菜單以及下面的子菜單;子菜單:只刪除子菜單
            string sqlDelete = string.Empty;
            if (!isMainMenu)
                sqlDelete = "DELETE F_TREE WHERE ID=:ID"; //子菜單
            else
                sqlDelete = "DELETE F_TREE WHERE ID IN(SELECT ID FROM F_TREE WHERE PARENTID IN(@ID)) OR ID=@ID"; //主菜單
            List<object> lps = new List<object>();
            SqlParameter spID = new SqlParameter("@ID", string.IsNullOrEmpty("") ? null : "");

            lps.Add(spID);

            //取得影响行数
            try
            {
                DelRights("");
                _effRows = iDal.OptSqlWithParameters(sqlDelete, lps);
                _code = 0;
                _msg = "Success:影響" + _effRows + "行";
            }
            catch (Exception ex)
            {
                _code = 1;
                _msg = "Fail:" + ex.ToString();//此處可以定義成你自己的報錯信息
            }
            LogHelper.WriteSqlDbLog("用户[" + UserInfo.EmpNo + "]" + UserInfo.EUserName + "菜单权限编辑操作:" + _msg + "!");
            return new MyJsonResult() { code = _code, msg = _msg };
        }
    }
}
