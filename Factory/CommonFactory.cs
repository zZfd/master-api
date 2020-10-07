using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Factory
{
    public static class CommonFactory
    {
        /// <summary>
        /// 格式化进球
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static string ScoreFlagFormat(short flag)
        {
            Dictionary<short, string> scoreFlag = new Dictionary<short, string>
            {
                { 0,"运动战进球"},
                {1,"定位球" },
                {2,"角球" },
                {3,"乌龙球" },
                {4,"点球" }
            };
            try
            {
                return scoreFlag[flag];
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 格式化球员状态
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string PlayerStatusFormat(short status)
        {
            Dictionary<short, string> statusMap = new Dictionary<short, string>
            {
                { 0,"正常"},
                {1,"禁赛" },
                {-1,"已退役" }
            };
            try
            {
                return statusMap[status];
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 格式化球员位置
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static string PlayerFlagFormat(short flag)
        {
            Dictionary<short, string> playerFlag = new Dictionary<short, string>
            {
                { 0,"守门员"},
                {1,"前场" },
                {2,"中场" },
                {3,"后场" }
            };
            try
            {
                return playerFlag[flag];
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 正常、禁用、已删除
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string NormalStatusFormat(short status)
        {
            Dictionary<short, string> normalStatus = new Dictionary<short, string>
            {
                { 0,"正常"},
                {1,"禁用" },
                {-1,"已删除" }
            };
            try
            {
                return normalStatus[status];
            }
            catch
            {
                return "";
            }
        }
    }
}