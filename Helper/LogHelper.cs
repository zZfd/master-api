using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public static class LogHelper
    {
        public static void WriteErrorLog(Exception ex)
        {
            try
            {
                if (!System.IO.Directory.Exists(Directory.GetCurrentDirectory()+ @"\Logs"))
                {
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Logs");
                }
                StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + @"\Logs" + DateTime.Now.ToString("yyyyMMdd") + "_Error_Log.ini", true);
                sw.WriteLine("================================  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  ================================");
                sw.WriteLine("Exception Message:" + ex.Message);
                sw.WriteLine("Exception Source:" + ex.Source);
                if (ex.InnerException != null) { sw.WriteLine("Exception InnerException:" + ex.InnerException.Message); }
                sw.WriteLine("Exception StackTrace:");
                sw.WriteLine(ex.StackTrace);
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
