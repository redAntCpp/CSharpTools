using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace HOTApi.Lib
{
    public class OracleHelper
    {
        public string ConnetionString;

        public OracleHelper()
        {
            string hostaddress = "192.168.50.97";
            string servername = "ORCL";
            string uid = "system";
            string pwd = "admin";
            string port = "31521";


            ConnetionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=" + hostaddress + ")" +
                "(PORT=" + port + "))(CONNECT_DATA=(SERVICE_NAME=" + servername + ")));" +
                "Persist Security Info=True;" +
                "User ID=" + uid + ";" +
                "Password=" + pwd + ";";
        }

        public bool IsConnect()
        {
            using (OracleConnection conn = new OracleConnection(ConnetionString))
            {
                try
                {
                    conn.Open();
                    Log.AddTrack("连接成功！", "123");
                    return true;
                }
                catch (OracleException oex)
                {
                    Log.AddTrack("连接失败！", oex.Message);
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public DataSet SelectData(string sqlStr)
        {
            Log.AddInfo("HOTApi.Lib.OracleHelper.SelectData", "Begin");
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnetionString))
                {
                    OracleDataAdapter sda = new OracleDataAdapter(sqlStr, conn);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    Log.AddInfo("HOTApi.Lib.OracleHelper.SelectData", "End");
                    return ds;
                }
            }
            catch (Exception SqlEx)
            {
                Log.AddError("HOTApi.Lib.OracleHelper.SelectData", "执行Sql异常：" + SqlEx.Message);
                throw SqlEx;
            }
        }

        /// <summary>
        /// 连接数据库，并选择数据，显示在dataset
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="ConnetStr">连接字符串</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>结果集</returns>
        public DataSet SelectData(string sqlStr, params OracleParameter[] parameters)
        {
            Log.AddInfo("HOTApi.Lib.OracleHelper.SelectData", "Begin");
            using (OracleConnection conn = new OracleConnection(ConnetionString))
            {
                conn.Open();
                DataSet ds = new DataSet();
                using (OracleCommand cmd = conn.CreateCommand())
                {
                    try
                    {
                        cmd.CommandText = sqlStr;
                        cmd.Parameters.AddRange(parameters);
                        Log.AddTrack("HOTApi.Lib.OracleHelper.SelectData", "执行sql：" + cmd.CommandText.ToString());
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        adapter.Fill(ds);//将适配器内容填充到dataset
                        Log.AddInfo("HOTApi.Lib.OracleHelper.SelectData", "End");
                        return ds;
                    }
                    catch (OracleException SqlEx)
                    {
                        Log.AddError("HOTApi.Lib.OracleHelper.SelectData", "执行Sql异常：" + SqlEx.Message);
                        throw SqlEx;
                    }
                }
            }

        }

        /// <summary>
        /// 选择某些数据，但不返回具体值，可以方便核实数据是否存在，比如count（*）
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="ConnetStr">连接字符串</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>受影响的行数，注意，只返回第一行第一列</returns>
        public int SelectDataNonQuery(string sqlStr, params OracleParameter[] parameters)
        {
            Log.AddInfo("HOTApi.Lib.OracleHelper.SelectDataNonQuery", "Begin");
            using (OracleConnection conn = new OracleConnection(ConnetionString))
            {
                conn.Open();
                DataSet ds = new DataSet();
                using (OracleCommand cmd = conn.CreateCommand())
                {
                    try
                    {
                        cmd.CommandText = sqlStr;
                        cmd.Parameters.AddRange(parameters);
                        Log.AddTrack("HOTApi.Lib.OracleHelper.SelectDataNonQuery", "执行sql：" + cmd.CommandText.ToString());
                        int returnvalue = (int)cmd.ExecuteScalar();
                        return returnvalue; //受影响行数
                    }
                    catch (OracleException SqlEx)
                    {
                        throw SqlEx;
                    }
                }
            }
        }

        DataSet ExecSelectStoredProcedure(string procName, params OracleParameter[] parameters)
        {
            Log.AddTrack("HOTApi.Lib.OracleHelper.ExecSelectStoredProcedure", "Begin");
            using (OracleConnection conn = new OracleConnection(ConnetionString))
            {
                conn.Open();
                DataSet ds = new DataSet();
                using (OracleCommand cmd = conn.CreateCommand())
                {
                    try
                    {
                        cmd.CommandText = procName;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(parameters);
                        Log.AddTrack("HOTApi.Lib.OracleHelper.ExecSelectStoredProcedure", "执行存储过程：" + procName);
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        adapter.Fill(ds);//将适配器内容填充到dataset
                        Log.AddTrack("HOTApi.Lib.OracleHelper.ExecSelectStoredProcedure", "End");
                        return ds;
                    }
                    catch (OracleException SqlEx)
                    {
                        Log.AddError("HOTApi.Lib.OracleHelper.ExecSelectStoredProcedure", "执行存储过程" + procName + "异常：" + SqlEx.Message);
                        throw SqlEx;
                    }
                }
            }
        }

        /// <summary>
        /// 执行sql语句，从数据库中修改数据（含增、删、查、改），
        /// </summary>
        /// <param name="sqlStr">sql语句字符串</param>
        /// <returns>受影响的行数</returns>
        public int ChangeData(string sqlStr)
        {
            Log.AddTrack("HOTApi.Lib.OracleHelper.ChangeData", "Begin");
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnetionString))
                {
                    conn.Open();
                    OracleCommand cmd = conn.CreateCommand();
                    cmd.CommandText = sqlStr;
                    Log.AddTrack("HOTApi.Lib.OracleHelper.ChangeData", "执行Oracle语句：" + cmd.CommandText);
                    int i = cmd.ExecuteNonQuery();
                    Log.AddTrack("HOTApi.Lib.OracleHelper.ChangeData", "End");
                    return i;
                }
            }
            catch (OracleException SqlEx)
            {
                Log.AddError("HOTApi.Lib.OracleHelper.ChangeData", "执行Sql异常：" + SqlEx.Message);
                throw SqlEx;
            }
        }
        /// <summary>
        /// 重载函数：
        /// 修改数据库数据，采用参数化执行，示例如下
        /// SqlParameter[] par = {new SqlParameter("@n", name),//关联
        ///                        new SqlParameter("@pwd", pwd)
        ///                        };
        ///string sql = "select count(*) from dbo.student  where studentno =  "+ name +" and loginpwd = '"+pwd +"'";
        ///SqlCommand cmd = new SqlCommand(sql, con);
        ///cmd.Parameters.AddRange(par);//添加参数化数组到cmd中
        /// </summary>
        /// <param name="sqlStr">需要执行的sql语句，含参</param>
        /// <param name="parameters">参数化数组</param>
        /// <returns>受影响的行数</returns>
        public int ChangeData(string sqlStr, params OracleParameter[] parameters)
        {
            Log.AddTrack("HOTApi.Lib.OracleHelper.changeData", "Begin");
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnetionString))
                {
                    conn.Open();
                    OracleCommand cmd = new OracleCommand(sqlStr, conn);//conn.CreateCommand();
                    cmd.Parameters.AddRange(parameters);////添加参数化数组到cmd中
                    Log.AddTrack("HOTApi.Lib.OracleHelper.changeData", "执行sql：" + cmd.CommandText);
                    int i = cmd.ExecuteNonQuery();
                    Log.AddTrack("HOTApi.Lib.OracleHelper.changeData", "End");
                    return i;
                }
            }
            catch (OracleException SqlEx)
            {
                Log.AddError("HOTApi.Lib.OracleHelper.changeData", "执行Sql异常：" + SqlEx.Message);
                throw SqlEx;
            }
        }
        /// <summary>
        /// 执行存储过程，使用事务处理，若只要有一条不通过全部回滚，避免脏数据的产生
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="parameters">参数数组，参考用例见上</param>
        /// <returns>受影响的行数</returns>
        public int ExecChangeStoredProcedure(string procName, params OracleParameter[] parameters)
        {
            Log.AddTrack("HOTApi.Lib.OracleHelper.ExecChangeStoredProcedure", "Begin");
            int rows;
            using (OracleConnection conn = new OracleConnection(ConnetionString))
            {
                conn.Open();
                using (OracleCommand cmd = conn.CreateCommand())
                {
                    //开始事务
                    OracleTransaction transaction = conn.BeginTransaction();
                    cmd.Transaction = transaction;
                    try
                    {
                        cmd.CommandText = procName;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(parameters);
                        Log.AddTrack("HOTApi.Lib.OracleHelper.ExecChangeStoredProcedure", "执行存储过程：" + procName);
                        rows = cmd.ExecuteNonQuery();
                        transaction.Commit(); //当上述执行没有发生异常时，提交事务
                        Log.AddTrack("HOTApi.Lib.OracleHelper.ExecChangeStoredProcedure", "End");
                        return rows;
                    }
                    catch (OracleException SqlEx)
                    {
                        Log.AddError("HOTApi.Lib.OracleHelper.ExecChangeStoredProcedure", "执行存储过程：" + procName + "异常：" + SqlEx.Message);
                        Log.AddTrack("HOTApi.Lib.OracleHelper.ExecChangeStoredProcedure", "尝试回滚...");
                        transaction.Rollback();//事务执行异常，则回滚并抛出异常
                        Log.AddTrack("HOTApi.Lib.OracleHelper.ExecChangeStoredProcedure", "回滚完成");
                        throw SqlEx;
                    }
                }
            }

        }

    }
}