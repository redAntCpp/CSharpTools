using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*
 * Create by : redAnt
 * Description : 连接SqlServer，对ado操作进行封装
 * Create Time：2021年3月10日17:20:01
 * Update Time：2021年9月30日18:47:18
 * info:
       2021年3月10日:
	    1. 创建文件，封装基本操作
	   2021年9月30日
	    1. 添加私有变量，进一步封装操作
		2. 添加参数化查询以及操作
 */

namespace DBHelper
{
    class SqlServerHelper
    {
        private string ConnetionString;
        /// <summary>
        /// 创建一个SqlServer的连接字符串，用于建立SqlServer连接
        /// 通常作为全局变量
        /// </summary>
        /// <param name="DataSource">数据库服务器地址</param>
        /// <param name="InitialCatalog">库名</param>
        /// <param name="UserID">用户名</param>
        /// <param name="Password">密码</param>
        /// <returns></returns>
        public void buildConnetionString(string DataSource, string InitialCatalog, string UserID, string Password)
        {
            string connetStr = $"Data Source = {DataSource};"
                             + $"Initial Catalog = {InitialCatalog};"
                             + $"User ID = {UserID};"
                             + $"Password = {Password};";
            ConnetionString = connetStr;
        }


        /// <summary>
        /// 从数据库中修改数据（含增、删、查、改），可重载使用参数化
        /// </summary>
        /// <returns>受影响的行数</returns>
        public int changeData(string sqlStr)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnetionString))
                {
                    conn.Open();
                    SqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = sqlStr;
                    int i = cmd.ExecuteNonQuery();
                    return i;
                }
            }
            catch (SqlException SqlEx)
            {
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
        /// <param name="ConnetionString">连接字符串</param>
        /// <param name="parameters">参数化数组</param>
        /// <returns></returns>
        public int changeData(string sqlStr, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnetionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sqlStr, conn);//conn.CreateCommand();
                    cmd.Parameters.AddRange(parameters);////添加参数化数组到cmd中
                    int i = cmd.ExecuteNonQuery();
                    return i;
                }
            }
            catch (SqlException SqlEx)
            {
                throw SqlEx;
            }
        }

        /// <summary>
        /// 执行存储过程，使用事务处理，若只要有一条不通过全部回滚，避免脏数据的产生
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="ConnetionString">连接字符串</param>
        /// <param name="parameters">参数数组，参考用例见上</param>
        /// <returns></returns>
        public int ExecChangeStoredProcedure(string procName, params SqlParameter[] parameters)
        {
            int rows = 0;
            using (SqlConnection conn = new SqlConnection(ConnetionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //开始事务
                    SqlTransaction transaction = conn.BeginTransaction();
                    cmd.Transaction = transaction;
                    try
                    {
                        cmd.CommandText = procName;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(parameters);
                        rows = cmd.ExecuteNonQuery();
                        transaction.Commit(); //当上述执行没有发生异常时，提交事务
                        return rows;
                    }
                    catch (SqlException sqlex)
                    {
                        transaction.Rollback();//事务执行异常，则回滚并抛出异常
                        throw sqlex;
                    }
                }
            }

        }

        /// <summary>
        /// 执行存储过程，选择所需要的数据
        /// 此存储过程对应需要参数的查询
        /// </summary>
        /// <returns>存储过程返回的结果集</returns>
        DataSet ExecSelectStoredProcedure(string procName, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(ConnetionString))
            {
                conn.Open();
                DataSet ds = new DataSet();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    try
                    {
                        cmd.CommandText = procName;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(parameters);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(ds);//将适配器内容填充到dataset
                        return ds;
                    }
                    catch (SqlException sqlex)
                    {
                        throw sqlex;
                    }
                }
            }
        }
        /// <summary>
        /// 选择某些数据，但不返回具体值，可以方便核实数据是否存在，比如count（*）
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>受影响的行数，注意，只返回第一行第一列</returns>
        public int selectDataNonQuery(string sqlStr, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(ConnetionString))
            {
                conn.Open();
                DataSet ds = new DataSet();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    try
                    {
                        cmd.CommandText = sqlStr;
                        cmd.Parameters.AddRange(parameters);
                        int returnvalue = (int)cmd.ExecuteScalar();
                        return returnvalue; //受影响行数
                    }
                    catch (SqlException sqlex)
                    {
                        throw sqlex;
                    }
                }
            }
        }

        
        /// <summary>
        /// 从数据库中选择数据
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <returns></returns>
        public DataSet selectDate(string sqlStr)
        {
            using (SqlConnection conn = new SqlConnection(ConnetionString))
            {
                conn.Open();
                DataSet ds = new DataSet();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    try
                    {
                        cmd.CommandText = sqlStr;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(ds);//将适配器内容填充到dataset
                        return ds;
                    }
                    catch (SqlException sqlex)
                    {
                        throw sqlex;
                    }
                }
            }
        }
		
		
        /// <summary>
        /// 重载selectData，添加参数化数组到
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public DataSet selectData(string sqlStr, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(ConnetionString))
            {
                conn.Open();
                DataSet ds = new DataSet();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    try
                    {
                        cmd.CommandText = sqlStr;
                        cmd.Parameters.AddRange(parameters);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(ds);//将适配器内容填充到dataset
                        return ds;
                    }
                    catch (SqlException sqlex)
                    {
                        throw sqlex;
                    }
                }
            }
        }

    }
}
