using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBHelper
{
    class MySqlHelper
    {
        /// <summary>
        /// 创建一个MySql的连接字符串，用于建立MySql连接
        /// 通常作为全局变量
        /// </summary>
        /// <param name="server">服务器地址</param>
        /// <param name="port">端口号</param>
        /// <param name="user">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="database">数据库名</param>
        /// <returns></returns>
        public string buildConnetionString(string server, string port, string user, string password, string database)
        {
            string ConnectMysqlString = $"server={server};" +
                                        $"port={port};" +
                                        $"user={user};" +
                                        $"password={password};" +
                                        $"database={database};";
            return ConnectMysqlString;
        }
        /// <summary>
        /// 测试数据库连接
        /// </summary>
        /// <param name="connectString">连接字符串</param>
        /// <returns>返回是否成功</returns>
        private bool checkDBConnect(string connectString)
        {
            MySqlConnection conn = new MySqlConnection(connectString);
            try
            {
                conn.Open();//打开通道，建立连接，可能出现异常,使用try catch语句
                return true;
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();//测试完就关闭
            }
        }
        /// <summary>
        /// 从数据库中选择数据集，直接查询数据集
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="connetString">连接字符串</param>
        /// <returns>返回结果集</returns>
        public DataSet selectData(string sqlStr, string connetString)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connetString))
                {
                    MySqlDataAdapter sda = new MySqlDataAdapter(sqlStr, conn);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    return ds;
                }
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 从数据库中选择数据集,重载函数，采用参数化，防止sql注入
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="connetString">连接字符串</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        public DataSet selectData(string sqlStr, string connetString, params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = new MySqlConnection(connetString))
            {
                conn.Open();
                DataSet ds = new DataSet();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    try
                    {
                        cmd.CommandText = sqlStr;
                        cmd.Parameters.AddRange(parameters);//填充参数
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        adapter.Fill(ds);//将适配器内容填充到dataset
                        return ds;
                    }
                    catch (MySqlException sqlex)
                    {
                        throw sqlex;
                    }
                }
            }
        }

        /// <summary>
        /// 执行存储过程，返回结果集
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="connetString">连接字符串</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        DataSet ExecSelectStoredProcedure(string procName, string connetString, params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = new MySqlConnection(connetString))
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
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        adapter.Fill(ds);//将适配器内容填充到dataset
                        return ds;
                    }
                    catch (MySqlException sqlex)
                    {
                        throw sqlex;
                    }
                }
            }
        }
    }
}
