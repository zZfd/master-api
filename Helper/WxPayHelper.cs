using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Helper
{
    /// <summary>
    /// 微信支付
    /// </summary>
    public static class WxPayHelper
    {
        #region 生成签名
        /// <summary>
        /// 获取签名数据
        ///</summary>
        /// <param name="strParam"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetSignInfo(Dictionary<string, string> strParam, string key)
        {
            int i = 0;
            string sign = string.Empty;
            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (KeyValuePair<string, string> temp in strParam)
                {
                    if (temp.Value == "" || temp.Value == null || temp.Key.ToLower() == "sign")
                    {
                        continue;
                    }
                    i++;
                    sb.Append(temp.Key.Trim() + "=" + temp.Value.Trim() + "&");
                }
                sb.Append("key=" + key.Trim() + "");
                sign = CreateMD5Hash(sb.ToString(), Encoding.UTF8).ToUpper();
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex, "PayHelper GetSignInfo");
            }
            return sign;
        }
        #endregion

        #region XML 处理
        /// <summary>
        /// 获取XML值
        /// </summary>
        /// <param name="strXml">XML字符串</param>
        /// <param name="strData">字段值</param>
        /// <returns></returns>
        public static string GetXmlValue(string strXml, string strData)
        {
            string xmlValue = string.Empty;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(strXml);
            var selectSingleNode = xmlDocument.DocumentElement.SelectSingleNode(strData);
            if (selectSingleNode != null)
            {
                xmlValue = selectSingleNode.InnerText;
            }
            return xmlValue;
        }

        /// <summary>
        /// 集合转换XML数据 (拼接成XML请求数据)
        /// </summary>
        /// <param name="strParam">参数</param>
        /// <returns></returns>
        public static string CreateXmlParam(Dictionary<string, string> strParam)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("<xml>");
                foreach (KeyValuePair<string, string> k in strParam)
                {
                    if (k.Key == "attach" || k.Key == "body" || k.Key == "sign")
                    {
                        sb.Append("<" + k.Key + "><![CDATA[" + k.Value + "]]></" + k.Key + ">");
                    }
                    else
                    {
                        sb.Append("<" + k.Key + ">" + k.Value + "</" + k.Key + ">");
                    }
                }
                sb.Append("</xml>");
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog( ex, "PayHelper CreateXmlParam");
            }

            return sb.ToString();
        }

        /// <summary>
        /// XML数据转换集合（XML数据拼接成字符串)
        /// </summary>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetFromXml(string xmlString)
        {
            Dictionary<string, string> sParams = new Dictionary<string, string>();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlString);
                XmlElement root = doc.DocumentElement;
                int len = root.ChildNodes.Count;
                for (int i = 0; i < len; i++)
                {
                    string name = root.ChildNodes[i].Name;
                    if (!sParams.ContainsKey(name))
                    {
                        sParams.Add(name.Trim(), root.ChildNodes[i].InnerText.Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex, "PayHelper GetFromXml");

            }
            return sParams;
        }

        /// <summary>
        /// 返回通知 XML
        /// </summary>
        /// <param name="returnCode"></param>
        /// <param name="returnMsg"></param>
        /// <returns></returns>
        public static string GetReturnXml(string returnCode, string returnMsg)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<xml>");
            sb.Append("<return_code><![CDATA[" + returnCode + "]]></return_code>");
            sb.Append("<return_msg><![CDATA[" + returnMsg + "]]></return_msg>");
            sb.Append("</xml>");
            return sb.ToString();
        }
        #endregion

        #region 生成MD5码
        public static string CreateMD5Hash(string str,Encoding encoding)
        {
            string pwd = "";
            MD5 md5 = MD5.Create();//实例化一个md5对像
                                   // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(encoding.GetBytes(str));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符
                pwd += s[i].ToString("x2");
            }
            return pwd;
        }

        #endregion
    }

}
