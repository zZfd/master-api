using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public static class TypeHelper
    {
        /// <summary>
        /// 将字符串guid转为Guid
        /// 失败返回Guid.Empty
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Guid ToGuid(string id)
        {
            try
            {
                return Guid.Parse(id);
            }
            catch
            {
                return Guid.Empty;
            }
        }
        /// <summary>
        /// 时间戳从1970年1月1日00:00:00至今的秒数,即当前的时间
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            // 时间戳从1970年1月1日00:00:00至今的秒数,即当前的时间
            long origin = new DateTime(1970, 1, 1).Second;
            //13位时间戳 与js时间戳保持一致，精确到毫秒
            string now = ((DateTime.Now.ToLocalTime().Second - origin)).ToString();
            return now;
        }
    }
}
