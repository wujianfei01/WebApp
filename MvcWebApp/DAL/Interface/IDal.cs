using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;

namespace DAL.Interface
{
    public abstract class IDal
    {
        public static IDal InstanceOracle(string cKey)
        {
            return new OracleDal(cKey);
        }

        public static IDal InstanceSqlServer(string cKey)
        {
            return new SqlServerDal(cKey);
        }

        /// <summary>
        /// oracle連接字符串
        /// </summary>
        public static string CnnectionDefault(string connKey)
        {
            return ConfigurationManager.ConnectionStrings[connKey].ToString();
        }

        /// <summary>
        /// 查詢
        /// </summary>
        /// <param name="sql">sql語句</param>
        /// <returns></returns>
        public abstract DataSet QuerySql(string sql);

        /// <summary>
        /// 查詢
        /// </summary>
        /// <param name="sql">sql語句with parameters</param>
        /// <param name="ParList">List of OracleParameter</param>
        /// <returns></returns>
        public abstract DataSet QuerySqlWithParameters(string sql, List<object> ParList);

        /// <summary>
        /// 操作語句
        /// </summary>
        /// <returns>影響行數</returns>
        public abstract int OptSql(string sql);

        /// <summary>
        /// 操作語句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public abstract int OptSql(string sql, object conn);

        /// <summary>
        /// 操作語句WithParameters
        /// </summary>
        /// <param name="sql">sql語句with parameters</param>
        /// <param name="ParList">List of OracleParameter</param>
        /// <returns></returns>
        public abstract int OptSqlWithParameters(string sql, List<object> ParList);

        /// <summary>
        /// 操作數據 With Parameters And Transaction
        /// 在調用此方法前打開事務
        /// XXXConnection conn = new XXXConnection(IDal.CnnectionDefault(connKey));
        /// XXXTransaction ort = conn.BeginTransaction();
        /// Your Code
        /// ort.Commit();
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="ParList"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public abstract int OptSqlWithParameters(string sql, List<object> ParList, object conn);
    }
}
