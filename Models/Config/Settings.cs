﻿using System;
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
}