using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.Response.Mini
{
    public class Member
    {
    }
    // 微信登录之后返回的对象
    public class WxLogin
    {
        // 用户唯一标识
        public string openid { get; set; }
        // 会话密钥
        public string session_key { get; set; }
        // 用户在开放平台的唯一标识符，在满足 UnionID 下发条件的情况下会返回
        public string unionid { get; set; }
        // 错误码
        public int errcode { get; set; }
        // 错误信息
        public string errmsg { get; set; }
    }

    public class WxUserInfo
    {
        public string openId { get; set; }
        public string nickName { get; set; }
        public short gender { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string country { get; set; }
        public string avatarUrl { get; set; }
        public string unionId { get; set; }
        public Watermark watermark { get; set; }
    }
    public class Watermark
    {
        public string appid { get; set; }
        public int timestamp { get; set; }
    }

}