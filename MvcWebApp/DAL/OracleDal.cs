using DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;

namespace DAL
{
    public class OracleDal : IDal
    {
        private string connStr;
        public OracleDal(string connKey)
        {
            connStr = IDal.CnnectionDefault(connKey);
        }
        /// <summary>
        /// 準備command之參數
        /// </summary>
        /// <param name="cmd">commad對象</param>
        /// <param name="conn">connection對象</param>
        /// <param name="trans">transaction對象</param>
        /// <param name="cmdType">選項enum commandType</param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms">參數list</param>
        private void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans,
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
                foreach (OracleParameter parm in cmdParms)
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
            using (OracleConnection conn = new OracleConnection(connStr))
            {
                OracleCommand cmd = new OracleCommand();
                //cmd.BindByName = true;
                OracleDataAdapter adpt = new OracleDataAdapter();
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
            using (OracleConnection conn = new OracleConnection(connStr))
            {
                OracleCommand cmd = new OracleCommand();
                //cmd.BindByName = true;
                OracleDataAdapter adpt = new OracleDataAdapter();
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
            OracleCommand cmd = new OracleCommand();
            //cmd.BindByName = true;
            using (OracleConnection conn = new OracleConnection(connStr))
            {
                PrepareCommand(cmd, conn, null, CommandType.Text, sql, null);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public override int OptSqlWithParameters(string sql, List<object> ParList)
        {
            OracleCommand cmd = new OracleCommand();
            //cmd.BindByName = true;
            using (OracleConnection conn = new OracleConnection(connStr))
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
            OracleConnection orcc = conn as OracleConnection;
            OracleCommand cmd = new OracleCommand();
            //cmd.BindByName = true;

            PrepareCommand(cmd, orcc, null, CommandType.Text, sql, ParList);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;

        }

        public override int OptSql(string sql, object conn)
        {
            OracleCommand cmd = new OracleCommand();
            //cmd.BindByName = true;
            OracleConnection orcc = conn as OracleConnection;
            PrepareCommand(cmd, orcc, null, CommandType.Text, sql, null);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }
    }
}
