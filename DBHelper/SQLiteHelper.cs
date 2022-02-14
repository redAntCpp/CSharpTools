using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Web;

namespace HOTApi.Lib
{
    public class SQLiteHelper
    {
        private string ConnetionString;

        public void buildConnetionString(string DataSourcePath)
        {
            string connetStr = $"data source = {DataSourcePath};";
        }
        //SQLiteConnection dbConnection = new SQLiteConnection(connectionString);
        public bool CheckSQLiteConnect()
        {
            Log.AddInfo("HOTApi.Lib.SQLiteHelper.CheckSQLiteConnect()", "Begin");
            SQLiteConnection conn = new SQLiteConnection(ConnetionString);
            try
            {
                conn.Open();//打开通道，建立连接，可能出现异常,使用try catch语句
                Log.AddTrack("HOTApi.Lib.SQLiteHelper.CheckSQLiteConnect()", "连接数据库成功!");
                Log.AddInfo("HOTApi.Lib.SQLiteHelper.CheckSQLiteConnect()", "End");
                return true;
            }
            catch (SQLiteException ex)
            {
                Log.AddError("HOTApi.Lib.SQLiteHelper.CheckSQLiteConnect()", "连接数据库失败：\n" + ex.Message);
                return false;
            }
            finally
            {
                conn.Close();//测试完就关闭
            }
        }

        public DataSet SelectData(string sqlStr)
        {
            Log.AddInfo("HOTApi.Lib.SQLiteHelper.SelectData", "Begin");
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(ConnetionString))
                {
                    SQLiteDataAdapter sda = new SQLiteDataAdapter(sqlStr, conn);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    Log.AddInfo("HOTApi.Lib.SQLiteHelper.SelectData", "End");
                    return ds;
                }
            }
            catch (Exception SqlEx)
            {
                Log.AddError("HOTApi.Lib.SQLiteHelper.SelectData", "执行Sql异常：" + SqlEx.Message);
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
        public DataSet SelectData(string sqlStr, params SQLiteParameter[] parameters)
        {
            Log.AddInfo("HOTApi.Lib.SQLiteHelper.SelectData", "Begin");
            using (SQLiteConnection conn = new SQLiteConnection(ConnetionString))
            {
                conn.Open();
                DataSet ds = new DataSet();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    try
                    {
                        cmd.CommandText = sqlStr;
                        cmd.Parameters.AddRange(parameters);
                        Log.AddTrack("HOTApi.Lib.SQLiteHelper.SelectData", "执行sql：" + cmd.CommandText.ToString());
                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                        adapter.Fill(ds);//将适配器内容填充到dataset
                        Log.AddInfo("HOTApi.Lib.SQLiteHelper.SelectData", "End");
                        return ds;
                    }
                    catch (SQLiteException SqlEx)
                    {
                        Log.AddError("HOTApi.Lib.SQLiteHelper.SelectData", "执行Sql异常：" + SqlEx.Message);
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
        public int SelectDataNonQuery(string sqlStr, params SQLiteParameter[] parameters)
        {
            Log.AddInfo("HOTApi.Lib.SQLiteHelper.SelectDataNonQuery", "Begin");
            using (SQLiteConnection conn = new SQLiteConnection(ConnetionString))
            {
                conn.Open();
                DataSet ds = new DataSet();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    try
                    {
                        cmd.CommandText = sqlStr;
                        cmd.Parameters.AddRange(parameters);
                        Log.AddTrack("HOTApi.Lib.SQLiteHelper.SelectDataNonQuery", "执行sql：" + cmd.CommandText.ToString());
                        int returnvalue = (int)cmd.ExecuteScalar();
                        return returnvalue; //受影响行数
                    }
                    catch (SQLiteException SqlEx)
                    {
                        throw SqlEx;
                    }
                }
            }
        }

        DataSet ExecSelectStoredProcedure(string procName, params SQLiteParameter[] parameters)
        {
            Log.AddTrack("HOTApi.Lib.SQLiteHelper.ExecSelectStoredProcedure", "Begin");
            using (SQLiteConnection conn = new SQLiteConnection(ConnetionString))
            {
                conn.Open();
                DataSet ds = new DataSet();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    try
                    {
                        cmd.CommandText = procName;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(parameters);
                        Log.AddTrack("HOTApi.Lib.SQLiteHelper.ExecSelectStoredProcedure", "执行存储过程：" + procName);
                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                        adapter.Fill(ds);//将适配器内容填充到dataset
                        Log.AddTrack("HOTApi.Lib.SQLiteHelper.ExecSelectStoredProcedure", "End");
                        return ds;
                    }
                    catch (SQLiteException SqlEx)
                    {
                        Log.AddError("HOTApi.Lib.SQLiteHelper.ExecSelectStoredProcedure", "执行存储过程" + procName + "异常：" + SqlEx.Message);
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
            Log.AddTrack("HOTApi.Lib.SQLiteHelper.ChangeData", "Begin");
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(ConnetionString))
                {
                    conn.Open();
                    SQLiteCommand cmd = conn.CreateCommand();
                    cmd.CommandText = sqlStr;
                    Log.AddTrack("HOTApi.Lib.SQLiteHelper.ChangeData", "执行SQLite语句：" + cmd.CommandText);
                    int i = cmd.ExecuteNonQuery();
                    Log.AddTrack("HOTApi.Lib.SQLiteHelper.ChangeData", "End");
                    return i;
                }
            }
            catch (SQLiteException SqlEx)
            {
                Log.AddError("HOTApi.Lib.SQLiteHelper.ChangeData", "执行Sql异常：" + SqlEx.Message);
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
        public int ChangeData(string sqlStr, params SQLiteParameter[] parameters)
        {
            Log.AddTrack("HOTApi.Lib.SQLiteHelper.changeData", "Begin");
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(ConnetionString))
                {
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(sqlStr, conn);//conn.CreateCommand();
                    cmd.Parameters.AddRange(parameters);////添加参数化数组到cmd中
                    Log.AddTrack("HOTApi.Lib.SQLiteHelper.changeData", "执行sql：" + cmd.CommandText);
                    int i = cmd.ExecuteNonQuery();
                    Log.AddTrack("HOTApi.Lib.SQLiteHelper.changeData", "End");
                    return i;
                }
            }
            catch (SQLiteException SqlEx)
            {
                Log.AddError("HOTApi.Lib.SQLiteHelper.changeData", "执行Sql异常：" + SqlEx.Message);
                throw SqlEx;
            }
        }
        /// <summary>
        /// 执行存储过程，使用事务处理，若只要有一条不通过全部回滚，避免脏数据的产生
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="parameters">参数数组，参考用例见上</param>
        /// <returns>受影响的行数</returns>
        public int ExecChangeStoredProcedure(string procName, params SQLiteParameter[] parameters)
        {
            Log.AddTrack("HOTApi.Lib.SQLiteHelper.ExecChangeStoredProcedure", "Begin");
            int rows;
            using (SQLiteConnection conn = new SQLiteConnection(ConnetionString))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    //开始事务
                    SQLiteTransaction transaction = conn.BeginTransaction();
                    cmd.Transaction = transaction;
                    try
                    {
                        cmd.CommandText = procName;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(parameters);
                        Log.AddTrack("HOTApi.Lib.SQLiteHelper.ExecChangeStoredProcedure", "执行存储过程：" + procName);
                        rows = cmd.ExecuteNonQuery();
                        transaction.Commit(); //当上述执行没有发生异常时，提交事务
                        Log.AddTrack("HOTApi.Lib.SQLiteHelper.ExecChangeStoredProcedure", "End");
                        return rows;
                    }
                    catch (SQLiteException SqlEx)
                    {
                        Log.AddError("HOTApi.Lib.SQLiteHelper.ExecChangeStoredProcedure", "执行存储过程：" + procName + "异常：" + SqlEx.Message);
                        Log.AddTrack("HOTApi.Lib.SQLiteHelper.ExecChangeStoredProcedure", "尝试回滚...");
                        transaction.Rollback();//事务执行异常，则回滚并抛出异常
                        Log.AddTrack("HOTApi.Lib.SQLiteHelper.ExecChangeStoredProcedure", "回滚完成");
                        throw SqlEx;
                    }
                }
            }

        }


    }
}