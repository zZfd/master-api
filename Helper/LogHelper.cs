using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;


namespace Helper
{
    public static class LogHelper
    {
        /// <summary>
        /// 程序代码错误日志
        /// </summary>
        /// <param name="ex"></param>
        public static void WriteErrorLog(Exception ex,string url = "")
        {
            try
            {
                if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["MiniLogPath"] + "/ErrorLogs"))
                {
                    Directory.CreateDirectory(ConfigurationManager.AppSettings["LogPath"] + "/ErrorLogs");
                }
                StreamWriter sw = new StreamWriter(ConfigurationManager.AppSettings["MiniLogPath"] + "/ErrorLogs/" + DateTime.Now.ToString("yyyyMMdd") + "_Error_Log.ini", true);
                sw.WriteLine("================================  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  ================================");
                sw.WriteLine("Exception Message:" + ex.Message);
                sw.WriteLine("Exception Source:" + ex.Source);
                if (ex.InnerException != null) { sw.WriteLine("Exception InnerException:" + ex.InnerException.Message); }
                sw.WriteLine("Exception StackTrace:");
                sw.WriteLine(ex.StackTrace);
                sw.WriteLine("Exception Url:");
                sw.WriteLine(url);
                sw.WriteLine("=======================================================================================");
                sw.WriteLine("");
                sw.Close();//写入
            }
            finally
            {

            }
        }

        /// <summary>
        /// 权限验证失败日志
        /// </summary>
        /// <param name="msg"></param>
        public static void WriteUnAuthorizationLog(string msg)
        {
            try
            {
                if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["MiniLogPath"] + "/UnAuthorizationLogs"))
                {
                    Directory.CreateDirectory(ConfigurationManager.AppSettings["MiniLogPath"] + "/UnAuthorizationLogs");
                }
                StreamWriter sw = new StreamWriter(ConfigurationManager.AppSettings["MiniLogPath"] + "/UnAuthorizationLogs/" + DateTime.Now.ToString("yyyyMMdd") + "_UnAuthorization_Log.ini", true);
                sw.WriteLine("================================  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  ================================");
                sw.WriteLine(msg);
                sw.WriteLine("=======================================================================================");
                sw.WriteLine("");
                sw.Close();//写入
            }
            finally
            {

            }
        }

        /// <summary>
        /// 登录日志
        /// </summary>
        /// <param name="msg"></param>
        public static void WriteSinInLog(string msg)
        {
            try
            {
                if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["MiniLogPath"] + "/SignInLogs"))
                {
                    Directory.CreateDirectory(ConfigurationManager.AppSettings["MiniLogPath"] + "/SignInLogs");
                }
                StreamWriter sw = new StreamWriter(ConfigurationManager.AppSettings["MiniLogPath"] + "/SignInLogs/" + DateTime.Now.ToString("yyyyMMdd") + "_SignInLogs.ini", true);
                sw.WriteLine("================================  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  ================================");
                sw.WriteLine(msg);
                sw.WriteLine("=======================================================================================");
                sw.WriteLine("");
                sw.Close();//写入
            }
            finally
            {

            }
        }
    }
}
