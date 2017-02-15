using DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DAL
{
    public class SqlServerDal:IDal
    {
        private string connStr;
        public SqlServerDal(string connKey)
        {
            connStr = IDal.CnnectionDefault(connKey);
        }
        private void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans,
                                       CommandType cmdType, string cmdText, List<object> cmdParms)
        {

            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                    if (parm != null)
                    {
                        if (parm.Value == null)
                            parm.Value = DBNull.Value;
                        cmd.Parameters.Add(parm);
                    }
            }
        }

        public override DataSet QuerySql(string sql)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter adpt = new SqlDataAdapter();
                DataSet ds = new DataSet();
                PrepareCommand(cmd, conn, null, CommandType.Text, sql, null);
                adpt.SelectCommand = cmd;
                adpt.Fill(ds, "ResultTable");
                cmd.Parameters.Clear();
                cmd.Dispose();
                adpt.Dispose();
                return ds;
            }
        }

        public override DataSet QuerySqlWithParameters(string sql, List<object> ParList)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter adpt = new SqlDataAdapter();
                DataSet ds = new DataSet();
                PrepareCommand(cmd, conn, null, CommandType.Text, sql, ParList);
                adpt.SelectCommand = cmd;
                adpt.Fill(ds, "ResultTable");
                cmd.Parameters.Clear();
                cmd.Dispose();
                adpt.Dispose();
                return ds;
            }
        }



        public override int OptSql(string sql)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                PrepareCommand(cmd, conn, null, CommandType.Text, sql, null);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }


        /// <summary>
        /// 操作數據
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="ParList"></param>
        /// <returns></returns>
        public override int OptSqlWithParameters(string sql, List<object> ParList)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                PrepareCommand(cmd, conn, null, CommandType.Text, sql, ParList);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 操作數據 With Transaction
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="ParList"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public override int OptSqlWithParameters(string sql, List<object> ParList, object conn)
        {
            SqlConnection sqlcc = conn as SqlConnection;
            SqlCommand cmd = new SqlCommand();

            PrepareCommand(cmd, sqlcc, null, CommandType.Text, sql, ParList);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        public override int OptSql(string sql, object conn)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection sqlcc = conn as SqlConnection;
            PrepareCommand(cmd, sqlcc, null, CommandType.Text, sql, null);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }
    }
}
