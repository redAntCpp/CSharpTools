using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

//此文档作为出口需要用的函数
namespace LogLib
{
    public class LogHelper
    {

        //字符串数组常量定义（记住格式）//这个是从0开始，枚举是从一
        public static readonly string[] STR_EVENT_TYPE = new string[]
        {
            "其他",
            "错误",
            "信息",
            "追踪",
            "调试"
        };
        const string sAppLogFormat = "{0:HH:mm:ss},{1:G},{2},{3},{4}";
        //类型定义
        public enum LoggerType
        {
            logError = 1,
            logInfo,
            logTrack,
            logDebug
        };

        //属性定义

        //定义一个委托，委托类似于C++中指向函数的指针，即回调函数
        //返回任意一个参数为两个string，返回值为string的函数的返回值
        public delegate string CallBackSysLog(string ParamStr, string paramStr2);
        
        public int Level = 1;  //日志等级,默认只输出错误
        public string LogFileName  //日志文件名
        {
            get
            {
                return LogFileName;
            }
        }
        //私有成员
        private string FLogFileName;
        private static Mutex FLock;//定义一个互斥信号量
        private string LogPath;


        //构造函数1，从配置中读取写日志的地址
        /// <summary>
        /// 构造函数：自定义方式
        /// </summary>
        /// <param name="LogPath">存放日志文件的物理地址</param>
        /// <param name="SystemName">系统名称</param>
        public LogHelper(string LogPath, string SystemName)
        {
            createLog(LogPath, SystemName);
        }

        /// <summary>
        /// 构造函数2：.exe文件，
        /// </summary>
        /*
        public LogHelper LogHelperForExe()
        {
            //获取和设置当前目录（即该进程从中启动的目录）的完全限定路径。针对winform
            string LogPath = System.Windows.Forms.Application.StartupPath;//X:\xxx\xxx (.exe文件所在的目录)
            string SystemName = Path.GetFileNameWithoutExtension(System.Windows.Forms.Application.ExecutablePath);//获取文件名，不带拓展名
            //createLog(LogPath, SystemName);
            LogHelper lh = new LogHelper(LogPath, SystemName);
            return lh;
        }
        */
        

        //按日志等级输出
        public void AddTrack(string ProcedureName, string content)
        {
            if (Level >= 4)
            {
                AddLog(content, ProcedureName, LoggerType.logTrack);
            }
        }
        public void AddDebug(string ProcedureName, string content)
        {
            if (Level >= 3)
            {
                AddLog(content, ProcedureName, LoggerType.logDebug);
            }
        }
        public void AddInfo(string ProcedureName, string content)
        {
            if (Level >= 2)
            {
                AddLog(content, ProcedureName, LoggerType.logInfo);
            }
        }
        public void AddError(string ProcedureName, string content)
        {
            if (Level >= 1)
            {
                AddLog(content, ProcedureName, LoggerType.logError);
            }
        }

        //写入日志
        public bool AddLog(string Content, string ProcedureName = "", LoggerType LogType = LoggerType.logInfo)
        {
            AddTextToFile(ProcedureName, Content, LogType);
            return true;
        }
        public void AddTextToFile(string ProcedureName, string Text, LoggerType LogType)
        {
            int ProcessID;//无符号整型
            string fText;
            string fLocFileName;
            string fSource;
            try
            {
                ProcessID = Process.GetCurrentProcess().Id;
                fSource = ProcedureName;
                fLocFileName = FLogFileName;
                //枚举类转int，可加int直接转
                //下面的排列决定格式化输出的日志顺序
                fText = string.Format(sAppLogFormat, System.DateTime.Now, ProcessID, STR_EVENT_TYPE[(int)LogType], Text + " ", fSource);
                try
                {
                    FLock.WaitOne();//进入临界区域
                    File.AppendAllText(fLocFileName, fText + "\r\n"); //追加文本,并换行
                }
                finally
                {
                    FLock.ReleaseMutex();//离开临界区
                }
            }
            catch (Exception err)
            {
                File.AppendAllText(FLogFileName, "错误：写入日志时发生异常：" + err.Message + "\r\n");//可在日志中查看抛出的异常
                throw;//抛出异常
            }
        }

        private void createLog(string LogPath, string SystemName)
        {
            string LogDir = LogPath + "\\" + "Logs";//读取配置的地址
            if (!Directory.Exists(LogDir))
            {
                Directory.CreateDirectory(LogDir);  //目录不存在就动态创建
            }
            //日志文本的路径跟名称
            FLogFileName = LogDir + "\\" + SystemName + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            FLock = new Mutex();//新建一个互斥变量
        }
    }
}
