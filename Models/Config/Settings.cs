using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.Config
{
    public static class Status
    {
        //正常，启用
        public const short normal = 0;
        //异常，禁用
        public const short forbidden = 1;
        //已删除
        public const short deleted = -1;
    }

    public static class MenuType
    {
        //根节点
        public const short root = 0;
        //菜单
        public const short menu = 1;
        //页面
        public const short page = 2;
        //按钮
        public const short button = 3;
    }
    /// <summary>
    /// FT_Team Flag
    /// </summary>
    public static class FTeamFlag
    {
        //根节点
        public const short root = 0;
        //国家
        public const short country = 1;
        //联赛
        public const short league = 2;
        //球队
        public const short team = 3;
    }

    /// <summary>
    /// FT_Score Flag
    /// </summary>
    public static class FScoreFlag
    {
        //运动战进球
        public const short normal = 0;
        //定位球
        public const short set = 1;
        //角球
        public const short corner = 2;
        //乌龙球
        public const short own = 3;
        //点球
        public const short point = 4;
    }

    /// <summary>
    /// FT_Player Flag
    /// </summary>
    public static class FPlayerFlag
    {
        //守门员
        public const short keeper = 0;
        //前场
        public const short front = 1;
        //中场
        public const short middle = 2;
        //后场
        public const short back = 3;
    }
}