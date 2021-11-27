using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HOTApi.Lib
{

    //生成配置类
    public class HOTConfig
    {
        private static volatile IConfig config;
        private static object syncRoot = new object();

        public static IConfig GetConfig()
        {
            if (config == null)
            {
                lock (syncRoot)
                {
                    if (config == null)
                        config = new Config();
                }
            }
            return config;
        }
    }

    //具体配置项
    public class Config : IConfig
    {
        private JObject ConfigJson;
        public Config()
        {

            try
            {
                string HOTConfigPath = HostingEnvironment.MapPath(@"/HOTApiConfig.Json");
                // Log.AddTrack("读取配置文件的路径：HOTConfigPath:", HOTConfigPath);
                using (StreamReader file = System.IO.File.OpenText(HOTConfigPath))
                {
                    using (JsonTextReader reader = new JsonTextReader(file))
                    {
                        JObject o = (JObject)JToken.ReadFrom(reader);
                        //Log.AddTrack("读取配置文件：HOTConfig:", o.ToString());
                        ConfigJson = o;
                    }
                }
            }
            catch(Exception e)
            {
                throw e;
            }
            

        }

        //=======【证书路径设置】===================================== 
        /* 证书路径,注意应该填写绝对路径（仅退款、撤销订单时需要）
         * 1.证书文件不能放在web服务器虚拟目录，应放在有访问权限控制的目录中，防止被他人下载；
         * 2.建议将证书文件名改为复杂且不容易猜测的文件
         * 3.商户服务器要做好病毒和木马防护工作，不被非法侵入者窃取证书文件。
        */
        public string GetSSlCertPath()
        {
            return "";
        }
        public string GetSSlCertPassword()
        {
            return "";
        }
        //=======【上报信息配置】===================================
        /* 测速上报等级，0.关闭上报; 1.仅错误时上报; 2.全量上报
        */
        public int GetReportLevel()
        {
            return 1;
        }


        //=======【日志配置】===================================
        /* 日志等级，0.不输出日志；1.只输出错误信息; 2.输出错误和正常信息; 3.输出错误信息、正常信息和调试信息
           配置日志存放目录、日志名称
         */
        public int GetLogLevel()
        {

            int i = Convert.ToInt32(ConfigJson["LogConfig"]["LogLevel"]);
            return i;
           // return 4;

        }
        public string GetLogPath()
        {

            return ConfigJson["LogConfig"]["LogPath"].ToString();
           // return "D:\\API\\HOTApi";

        }

        public string GetLogName()
        {
            return ConfigJson["LogConfig"]["LogName"].ToString();//LogName
            //return "HOTApi";

        }


        //=========【数据库配置】===============================
        /*sql server数据库设计（ado.net）*/
        public string GetHOTConnetionString()
        {
            
            //配置
            string DataSource = "192.168.xxx";
            string InitialCatalog = "HOT";
            string UserID = "xxx";
            string Password = "xxx";
            //组装
            string ConnetionString = $"Data Source = {DataSource};"
                            + $"Initial Catalog = {InitialCatalog};"
                            + $"User ID = {UserID};"
                            + $"Password = {Password};";
            return ConnetionString;
            
            
        }
        public string GetHIS_OPConnetionString()
        {
            //配置
            string DataSource = "192.168.xxx.xxx";
            string InitialCatalog = "xx";
            string UserID = "xx";
            string Password = "xx";
            //组装
            string ConnetionString = $"Data Source = {DataSource};"
                            + $"Initial Catalog = {InitialCatalog};"
                            + $"User ID = {UserID};"
                            + $"Password = {Password};";
            return ConnetionString;
        }
    }




}