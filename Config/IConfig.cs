using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOTApi.Lib
{
    //公共配置接口
   public  interface IConfig
    {
        
        //=======【证书路径设置】===================================== 
        /* 证书路径,注意应该填写绝对路径（仅退款、撤销订单时需要）
         * 1.证书文件不能放在web服务器虚拟目录，应放在有访问权限控制的目录中，防止被他人下载；
         * 2.建议将证书文件名改为复杂且不容易猜测的文件
         * 3.商户服务器要做好病毒和木马防护工作，不被非法侵入者窃取证书文件。
        */
        string GetSSlCertPath();
        string GetSSlCertPassword();


        //=======【上报信息配置】===================================
        /* 测速上报等级，0.关闭上报; 1.仅错误时上报; 2.全量上报
        */
        int GetReportLevel();


        //=======【日志配置】===================================
        /* 日志等级，0.不输出日志；1.只输出错误信息; 2.输出错误和正常信息; 3.输出错误信息、正常信息和调试信息
           配置日志存放目录、日志名称
         */
        int GetLogLevel();
        string GetLogPath();
        string GetLogName();

        //=========【数据库配置】===============================
        /*sql server数据库设计（ado.net）*/
        string GetHOTConnetionString();
        string GetHIS_OPConnetionString();



    }
}
