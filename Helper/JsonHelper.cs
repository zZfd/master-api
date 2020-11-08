using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    /// <summary>
    /// Json帮助类
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// 将对象序列化为JSON格式
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string SerializeObject(object o)
        {
            string json = JsonConvert.SerializeObject(o);
            return json;
        }

        /// <summary>
        /// 解析JSON字符串生成对象实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T DeserializeJsonToObject<T>(string json) where T : class
        {
            try
            {
                JsonSerializer serializer = new JsonSerializer();
                System.IO.StringReader str = new System.IO.StringReader(json);
                object o = serializer.Deserialize(new JsonTextReader(str), typeof(T));
                T t = o as T;
                return t;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex);
                return default(T);
            }
        }

        /// <summary>
        /// 解析JSON数组生成对象实体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static List<T> DeserializeJsonToList<T>(string json) where T : class
        {
            try
            {
                JsonSerializer serializer = new JsonSerializer();
                System.IO.StringReader str = new System.IO.StringReader(json);
                object o = serializer.Deserialize(new JsonTextReader(str), typeof(List<T>));
                List<T> list = o as List<T>;
                return list;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex);
                return default(List<T>);
            }

        }

        /// <summary>
        /// 反序列化JSON到给定的匿名对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="anonymousTypeObject"></param>
        /// <returns></returns>
        public static T DeserializeJsonToAnonymous<T>(string json, T anonymousTypeObject)
        {
            try
            {
                T t = JsonConvert.DeserializeAnonymousType(json, anonymousTypeObject);
                return t;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex);
                return default(T);
            }

        }
    }
}
