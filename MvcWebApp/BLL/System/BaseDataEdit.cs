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
    public class BaseDataEdit
    {
        string connKey01 = "ConnectionStringOracle";
        private bool CheckIsExist(FormCollection collection)
        {
            IDal iDal = IDal.InstanceOracle(connKey01);
            DataSet ds = null;

            string sql = "SELECT 1 FROM F_BASE_DATA WHERE 1=1 ";
            if (!string.IsNullOrEmpty(collection["KEYS"]))
            {
                sql += "AND ID='{0}'";
                ds = iDal.QuerySql(string.Format(sql, collection["KEYS"]));
            }
            else if (collection["EORA"].Equals("a"))
                return false;
            else
                throw new ArgumentException("Error:系统无法取得Keys,请检查代码!");

            if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count.Equals(0))
                return false;
            else
                return true;
        }

        public MyJsonResult DisableBaseData(string keys,string opflag)
        {
            int _effRows = 0;
            int _code = 0;
            string _msg = string.Empty;

            string sqlUpdate = "UPDATE F_BASE_DATA SET ENABLE=:ENABLE WHERE ID=:ID";

            List<object> lps = new List<object>();
            OracleParameter ENABLE = new OracleParameter(":ENABLE", int.Parse(opflag));
            OracleParameter ID = new OracleParameter(":ID", keys);
            lps.Add(ENABLE);
            lps.Add(ID);
            IDal iDal = IDal.InstanceOracle(connKey01);

            try
            {
                _effRows = iDal.OptSqlWithParameters(sqlUpdate, lps);
                _code = 0;
                _msg = "Success:影響" + _effRows + "行";
            }
            catch (Exception ex)
            {
                _code = 1;
                _msg = "Fail:" + ex.ToString();
            }
            LogHelper.WriteSqlDbLog("用户[" + UserInfo.EmpNo + "]" + UserInfo.EUserName + "基础参数编辑操作:" + _msg + "!");
            return new MyJsonResult() { code = _code, msg = _msg };
        }
        public MyJsonResult SaveOrUpdate(FormCollection collection)
        {
            int _effRows = 0;
            int _code = 0;
            string _msg = string.Empty;
            string sqlInsert = "INSERT INTO F_BASE_DATA(BASE_NAME,BASE_CODE,BASE_TYPE,LAST_EDIT_BY,DESCRIPTION,ENABLE) VALUES(:BASE_NAME,:BASE_CODE,:BASE_TYPE,:LAST_EDIT_BY,:DESCRIPTION,0)";
            string sqlUpdate = "UPDATE F_BASE_DATA SET BASE_NAME=:BASE_NAME,BASE_CODE=:BASE_CODE,BASE_TYPE=:BASE_TYPE,LAST_EDIT_BY=:LAST_EDIT_BY,DESCRIPTION=:DESCRIPTION WHERE ID=:ID";
            string sql = CheckIsExist(collection) ? sqlUpdate : sqlInsert;
            List<object> lps = new List<object>();
            OracleParameter ID = new OracleParameter(":ID", string.IsNullOrEmpty(collection["KEYS"]) ? null : collection["KEYS"]);
            OracleParameter BASE_NAME = new OracleParameter(":BASE_NAME", string.IsNullOrEmpty(collection["BASE_NAME"]) ? null : collection["BASE_NAME"]);
            OracleParameter BASE_CODE = new OracleParameter(":BASE_CODE", string.IsNullOrEmpty(collection["BASE_CODE"]) ? null : collection["BASE_CODE"]);
            OracleParameter BASE_TYPE = new OracleParameter(":BASE_TYPE", string.IsNullOrEmpty(collection["BASE_TYPE"]) ? null : collection["BASE_TYPE"]);
            OracleParameter LAST_EDIT_BY = new OracleParameter(":LAST_EDIT_BY", UserInfo.UserName + "(" + UserInfo.EUserName + ")");
            OracleParameter DESCRIPTION = new OracleParameter(":DESCRIPTION", string.IsNullOrEmpty(collection["DESCRIPTION"]) ? null : collection["DESCRIPTION"]);

            lps.Add(BASE_NAME);
            lps.Add(BASE_CODE);
            lps.Add(BASE_TYPE);
            lps.Add(LAST_EDIT_BY);
            lps.Add(DESCRIPTION);

            if (CheckIsExist(collection))
                lps.Add(ID);

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
                _msg = "Fail:" + ex.ToString();
            }
            LogHelper.WriteSqlDbLog("用户[" + UserInfo.EmpNo + "]" + UserInfo.EUserName + "基础参数编辑操作:" + _msg + "!");
            return new MyJsonResult() { code = _code, msg = _msg };
        }

        public MyJsonResult Delete(params string[] keys)
        {
            int _effRows = 0;
            int _code = 0;
            string _msg = string.Empty;
            IDal iDal = IDal.InstanceOracle(connKey01);

            string sqlDelete = "DELETE F_BASE_DATA WHERE ID=:ID";
            List<object> lps = new List<object>();
            OracleParameter spID = new OracleParameter(":ID", string.IsNullOrEmpty(keys[0]) ? null : keys[0]);

            lps.Add(spID);
            try
            {
                _effRows = iDal.OptSqlWithParameters(sqlDelete, lps);
                _code = 0;
                _msg = "Success:影響" + _effRows + "行";
            }
            catch (Exception ex)
            {
                _code = 1;
                _msg = "Fail:" + ex.ToString();
            }
            LogHelper.WriteSqlDbLog("用户[" + UserInfo.EmpNo + "]" + UserInfo.EUserName + "基础参数删除操作!");
            return new MyJsonResult() { code = _code, msg = _msg };
        }

        public object GetComboxDataForBaseDataType()
        {
            List<JsonCombox> ls = new List<JsonCombox>();
            string sql = "SELECT DISTINCT BASE_TYPE AS ID,BASE_TYPE AS TEXT FROM F_BASE_DATA ";

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
    }

    public class BaseDataEditForSQLServer
    {
        string connKey01 = "ConnectionStringSqlServer";

        private bool CheckIsExist(FormCollection collection)
        {
            IDal iDal = IDal.InstanceSqlServer(connKey01);
            DataSet ds = null;

            string sql = "SELECT 1 FROM F_BASE_DATA WHERE 1=1 ";
            if (!string.IsNullOrEmpty(collection["KEYS"]))
            {
                sql += "AND ID='{0}'";
                ds = iDal.QuerySql(string.Format(sql, collection["KEYS"]));
            }
            else if (collection["EORA"].Equals("a"))
                return false;
            else
                throw new ArgumentException("Error:系统无法取得Keys,请检查代码!");

            if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count.Equals(0))
                return false;
            else
                return true;
        }
        public MyJsonResult DisableBaseData(string keys, string opflag)
        {
            int _effRows = 0;
            int _code = 0;
            string _msg = string.Empty;

            string sqlUpdate = "UPDATE F_BASE_DATA SET ENABLE=@ENABLE WHERE ID=@ID";

            List<object> lps = new List<object>();
            SqlParameter ID = new SqlParameter("@ID", keys);
            SqlParameter ENABLE = new SqlParameter("@ENABLE", int.Parse(opflag));
            lps.Add(ID);
            lps.Add(ENABLE);

            IDal iDal = IDal.InstanceSqlServer(connKey01);

            try
            {
                _effRows = iDal.OptSqlWithParameters(sqlUpdate, lps);
                _code = 0;
                _msg = "Success:影響" + _effRows + "行";
            }
            catch (Exception ex)
            {
                _code = 1;
                _msg = "Fail:" + ex.ToString();
            }
            LogHelper.WriteSqlDbLog("用户[" + UserInfo.EmpNo + "]" + UserInfo.EUserName + "基础参数编辑操作:" + _msg + "!");
            return new MyJsonResult() { code = _code, msg = _msg };
        }
        public MyJsonResult SaveOrUpdate(FormCollection collection)
        {
            int _effRows = 0;
            int _code = 0;
            string _msg = string.Empty;
            string sqlInsert = "INSERT INTO F_BASE_DATA(BASE_NAME,BASE_CODE,BASE_TYPE,LAST_EDIT_BY,DESCRIPTION,ENABLE) VALUES(@BASE_NAME,@BASE_CODE,@BASE_TYPE,@LAST_EDIT_BY,@DESCRIPTION,0)";
            string sqlUpdate = "UPDATE F_BASE_DATA SET BASE_NAME=@BASE_NAME,BASE_CODE=@BASE_CODE,BASE_TYPE=@BASE_TYPE,LAST_EDIT_BY=@LAST_EDIT_BY,DESCRIPTION=@DESCRIPTION WHERE ID=@ID";
            
            List<object> lps = new List<object>();
            SqlParameter ID = new SqlParameter("@ID", string.IsNullOrEmpty(collection["KEYS"]) ? null : collection["KEYS"]);
            SqlParameter BASE_NAME = new SqlParameter("@BASE_NAME", string.IsNullOrEmpty(collection["BASE_NAME"]) ? null : collection["BASE_NAME"]);
            SqlParameter BASE_CODE = new SqlParameter("@BASE_CODE", string.IsNullOrEmpty(collection["BASE_CODE"]) ? null : collection["BASE_CODE"]);
            SqlParameter BASE_TYPE = new SqlParameter("@BASE_TYPE", string.IsNullOrEmpty(collection["BASE_TYPE"]) ? null : collection["BASE_TYPE"]);
            SqlParameter LAST_EDIT_BY = new SqlParameter("@LAST_EDIT_BY", UserInfo.UserName + "(" + UserInfo.EUserName + ")");
            SqlParameter DESCRIPTION = new SqlParameter("@DESCRIPTION", string.IsNullOrEmpty(collection["DESCRIPTION"]) ? null : collection["DESCRIPTION"]);

            lps.Add(BASE_NAME);
            lps.Add(BASE_CODE);
            lps.Add(BASE_TYPE);
            lps.Add(LAST_EDIT_BY);
            lps.Add(DESCRIPTION);

            IDal iDal = IDal.InstanceSqlServer(connKey01);

            try
            {
                if (CheckIsExist(collection))
                    lps.Add(ID);

                string sql = CheckIsExist(collection) ? sqlUpdate : sqlInsert;

                _effRows = iDal.OptSqlWithParameters(sql, lps);
                _code = 0;
                _msg = "Success:影響" + _effRows + "行";
            }
            catch (Exception ex)
            {
                _code = 1;
                _msg = "Fail:" + ex.ToString();
            }
            return new MyJsonResult() { code = _code, msg = _msg };
        }



        public MyJsonResult Delete(params string[] keys)
        {
            int _effRows = 0;
            int _code = 0;
            string _msg = string.Empty;
            IDal iDal = IDal.InstanceSqlServer(connKey01);

            string sqlDelete = "DELETE F_BASE_DATA WHERE ID=@ID";
            List<object> lps = new List<object>();
            SqlParameter spID = new SqlParameter("@ID", string.IsNullOrEmpty(keys[0]) ? null : keys[0]);

            lps.Add(spID);
            try
            {
                _effRows = iDal.OptSqlWithParameters(sqlDelete, lps);
                _code = 0;
                _msg = "Success:影響" + _effRows + "行";
            }
            catch (Exception ex)
            {
                _code = 1;
                _msg = "Fail:" + ex.ToString();
            }
            LogHelper.WriteSqlDbLog("用户[" + UserInfo.EmpNo + "]" + UserInfo.EUserName + "基础参数删除操作!");
            return new MyJsonResult() { code = _code, msg = _msg };
        }

        public object GetComboxDataForBaseDataType()
        {
            List<JsonCombox> ls = new List<JsonCombox>();
            string sql = "SELECT DISTINCT BASE_TYPE AS ID,BASE_TYPE AS TEXT FROM F_BASE_DATA ";

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
