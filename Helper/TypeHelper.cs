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
    }
}
