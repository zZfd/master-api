using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public static class EncryptionHelper
    {
        /// <summary>  
        /// SHA1 加密，返回大写字符串  
        /// </summary>  
        /// <param name="content">需要加密字符串</param>  
        /// <param name="encode">指定加密编码</param>  
        /// <returns>返回40位大写字符串</returns>  
        public static string SHA1(string content, Encoding encode, bool upset = true)
        {
            try
            {
                SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
                byte[] bytes_in = encode.GetBytes(content);
                byte[] bytes_out = sha1.ComputeHash(bytes_in);
                sha1.Dispose();
                string result = BitConverter.ToString(bytes_out);
                result = result.Replace("-", "");

                if (upset)
                {
                    string str1 = result.Substring(8, 7);
                    string str2 = result.Substring(23, 7);
                    result = result.Remove(8, 7).Insert(8, str2);
                    result = result.Remove(23, 7).Insert(23, str1);
                    str1 = result.Substring(0, 10);
                    str2 = result.Substring(30, 10);
                    result = result.Remove(0, 10).Insert(0, str2);
                    result = result.Remove(30, 10).Insert(30, str1);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("SHA1加密出错：" + ex.Message);
            }
        }

        /// <summary>
        /// 生成token
        /// 由 首7 4 +3
        /// 体 
        /// 尾 13 4 + 9三部分组成
        /// 包括 nowTicks expires (userId,passwordSalt)加密字符串
        /// nowTicks 13位精确到毫秒
        /// expires 7位精确到毫秒
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="passwordSalt"></param>
        /// <param name="expires"></param>
        /// <returns></returns>
        public static string CreateToken(Guid userId, string passwordSalt, int expires = 19 * 58 * 1000)
        {
            string body = SHA1(userId.ToString("N") + passwordSalt, Encoding.UTF8, true);
            long origin = new DateTime(1970, 1, 1).Ticks;
            //13位时间戳 与js时间戳保持一致，精确到毫秒
            string now = ((DateTime.Now.ToUniversalTime().Ticks - origin) / 10000).ToString();
            //expires精确到毫秒，保证7位有效位 分秒毫秒
            string expriesStr = expires.ToString();
            string token = string.Format("{0}{1}{2}{3}{4}",
                now.Substring(0, 4), expriesStr.Substring(4), body,
                expriesStr.Substring(0, 4), now.Substring(4));
            return token;
        }

        /// <summary>
        /// 校验token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userId"></param>
        /// <param name="passwordSalt"></param>
        /// <returns></returns>
        public static bool CheckToken(string token, Guid userId, string passwordSalt)
        {
            try
            {
                string body = token.Substring(7, token.Length - 20);
                if (!body.Equals(SHA1(userId.ToString("N") + passwordSalt, Encoding.UTF8, true)))
                {
                    return false;
                }
                long origin = new DateTime(1970, 1, 1).Ticks;
                long time = long.Parse(token.Substring(0, 4) + token.Substring(token.Length - 9));
                long expires = long.Parse(token.Substring(token.Length - 13, 4) + token.Substring(4, 3));
                return new DateTime(origin + (time + expires) * 10000).ToLocalTime() > DateTime.Now;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex);
                return false;
            }
        }
    }
}
