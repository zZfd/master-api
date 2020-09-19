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
}