using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace HOTApi.Lib
{
    public class MySqlHelper
    {
        //连接字符串，不能为空
        public string ConnetionString;

        /// <summary>
        /// 检测数据库连接情况
        /// </summary>
        /// <returns></returns>
        public bool CheckMysqlConnect()
        {
            Log.AddInfo("HOTApi.Lib.MySqlHelper.CheckMysqlConnect()", "Begin");
            MySqlConnection conn = new MySqlConnection(ConnetionString);
            try
            {
                conn.Open();//打开通道，建立连接，可能出现异常,使用try catch语句
                Log.AddTrack("HOTApi.Lib.MySqlHelper.CheckMysqlConnect()", "连接数据库成功!");
                Log.AddInfo("HOTApi.Lib.MySqlHelper.CheckMysqlConnect()", "End");
                return true;
            }
            catch (MySqlException ex)
            {
                Log.AddError("HOTApi.Lib.MySqlHelper.CheckMysqlConnect()", "尝试连接数据库失败：\n" + ex.Message);
                //throw ex;
                return false;
            }
            finally
            {
                conn.Close();//测试完就关闭
            }

        }
        /// <summary>
        /// 执行sql语句，从数据库中修改数据（含增、删、查、改），
        /// </summary>
        /// <param name="sqlStr">sql语句字符串</param>
        /// <returns>受影响的行数</returns>
        public int ChangeData(string sqlStr)
        {
            Log.AddTrack("HOTApi.Lib.MySqlHelper.ChangeData", "Begin");
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnetionString))
                {
                    conn.Open();
                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = sqlStr;
                    Log.AddTrack("HOTApi.Lib.MySqlHelper.ChangeData", "执行mysql语句："+ cmd.CommandText);
                    int i = cmd.ExecuteNonQuery();
                    Log.AddTrack("HOTApi.Lib.MySqlHelper.ChangeData", "End");
                    return i;
                }
            }
            catch (MySqlException SqlEx)
            {
                Log.AddError("HOTApi.Lib.MySqlHelper.ChangeData", "执行Sql异常：" + SqlEx.Message);
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
        public int ChangeData(string sqlStr, params MySqlParameter[] parameters)
        {
            Log.AddTrack("HOTApi.Lib.MySqlHelper.changeData", "Begin");
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnetionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(sqlStr, conn);//conn.CreateCommand();
                    cmd.Parameters.AddRange(parameters);////添加参数化数组到cmd中
                    Log.AddTrack("HOTApi.Lib.MySqlHelper.changeData", "执行sql：" + cmd.CommandText);
                    int i = cmd.ExecuteNonQuery();
                    Log.AddTrack("HOTApi.Lib.MySqlHelper.changeData", "End");
                    return i;
                }
            }
            catch (MySqlException SqlEx)
            {
                Log.AddError("HOTApi.Lib.MySqlHelper.changeData", "执行Sql异常：" + SqlEx.Message);
                throw SqlEx;
            }
        }
        /// <summary>
        /// 执行存储过程，使用事务处理，若只要有一条不通过全部回滚，避免脏数据的产生
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="parameters">参数数组，参考用例见上</param>
        /// <returns>受影响的行数</returns>
        public int ExecChangeStoredProcedure(string procName, params MySqlParameter[] parameters)
        {
            Log.AddTrack("HOTApi.Lib.MySqlHelper.ExecChangeStoredProcedure", "Begin");
            int rows;
            using (MySqlConnection conn = new MySqlConnection(ConnetionString))
            {
                conn.Open();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    //开始事务
                    MySqlTransaction transaction = conn.BeginTransaction();
                    cmd.Transaction = transaction;
                    try
                    {
                        cmd.CommandText = procName;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(parameters);
                        Log.AddTrack("HOTApi.Lib.MySqlHelper.ExecChangeStoredProcedure", "执行存储过程："+ procName);
                        rows = cmd.ExecuteNonQuery();
                        transaction.Commit(); //当上述执行没有发生异常时，提交事务
                        Log.AddTrack("HOTApi.Lib.MySqlHelper.ExecChangeStoredProcedure", "End");
                        return rows;
                    }
                    catch (MySqlException SqlEx)
                    {
                        Log.AddError("HOTApi.Lib.MySqlHelper.ExecChangeStoredProcedure", "执行存储过程：" + procName +"异常：" + SqlEx.Message);
                        Log.AddTrack("HOTApi.Lib.MySqlHelper.ExecChangeStoredProcedure", "尝试回滚...");
                        transaction.Rollback();//事务执行异常，则回滚并抛出异常
                        Log.AddTrack("HOTApi.Lib.MySqlHelper.ExecChangeStoredProcedure", "回滚完成");
                        throw SqlEx;
                    }
                }
            }

        }
        /// <summary>
        /// 从数据库中选择数据
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <returns>结果集</returns>
        public DataSet SelectData(string sqlStr)
        {
            Log.AddTrack("HOTApi.Lib.MySqlHelper.SelectData", "Begin");
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnetionString))
                {
                    MySqlDataAdapter sda = new MySqlDataAdapter(sqlStr, conn);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    Log.AddTrack("HOTApi.Lib.MySqlHelper.SelectData", "End");
                    return ds;
                }
            }
            catch (Exception SqlEx)
            {
                Log.AddError("HOTApi.Lib.MySqlHelper.SelectData", "执行Sql异常：" + SqlEx.Message);
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
        public DataSet SelectData(string sqlStr, params MySqlParameter[] parameters)
        {
            Log.AddTrack("HOTApi.Lib.MySqlHelper.SelectData", "Begin");
            using (MySqlConnection conn = new MySqlConnection(ConnetionString))
            {
                conn.Open();
                DataSet ds = new DataSet();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    try
                    {
                        cmd.CommandText = sqlStr;
                        cmd.Parameters.AddRange(parameters);
                        Log.AddTrack("HOTApi.Lib.MySqlHelper.SelectData", "执行sql："+ cmd.CommandText.ToString());
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        adapter.Fill(ds);//将适配器内容填充到dataset
                        Log.AddTrack("HOTApi.Lib.MySqlHelper.SelectData", "End");
                        return ds;
                    }
                    catch (MySqlException SqlEx)
                    {
                        Log.AddError("HOTApi.Lib.MySqlHelper.SelectData", "执行Sql异常：" + SqlEx.Message);
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
        public int SelectDataNonQuery(string sqlStr, params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnetionString))
            {
                conn.Open();
                DataSet ds = new DataSet();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    try
                    {
                        cmd.CommandText = sqlStr;
                        cmd.Parameters.AddRange(parameters);
                        int returnvalue = (int)cmd.ExecuteScalar();
                        return returnvalue; //受影响行数
                    }
                    catch (MySqlException SqlEx)
                    {
                        throw SqlEx;
                    }
                }
            }
        }

        DataSet ExecSelectStoredProcedure(string procName, params MySqlParameter[] parameters)
        {
            Log.AddTrack("HOTApi.Lib.MySqlHelper.ExecSelectStoredProcedure", "Begin");
            using (MySqlConnection conn = new MySqlConnection(ConnetionString))
            {
                conn.Open();
                DataSet ds = new DataSet();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    try
                    {
                        cmd.CommandText = procName;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(parameters);
                        Log.AddTrack("HOTApi.Lib.MySqlHelper.ExecSelectStoredProcedure", "执行存储过程：" + procName);
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        adapter.Fill(ds);//将适配器内容填充到dataset
                        Log.AddTrack("HOTApi.Lib.MySqlHelper.ExecSelectStoredProcedure", "End");
                        return ds;
                    }
                    catch (MySqlException SqlEx)
                    {
                        Log.AddError("HOTApi.Lib.MySqlHelper.ExecSelectStoredProcedure", "执行存储过程" + procName +"异常："+ SqlEx.Message);
                        throw SqlEx;
                    }
                }
            }
        }


    }
}