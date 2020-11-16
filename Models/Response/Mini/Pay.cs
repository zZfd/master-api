using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.Response.Mini
{
    public class Pay
    {
        // 应用id
        public string appId { get; set; }
        // 时间戳从1970年1月1日00:00:00至今的秒数,即当前的时间
        public string timeStamp { get; set; }
        // 随机字符串，长度为32个字符以下。
        public string nonceStr { get; set; }
        // 统一下单接口返回的 prepay_id 参数值，提交格式如：prepay_id=*
        public string package { get; set; }
        // 签名类型，默认为MD5，支持HMAC-SHA256和MD5。注意此处需与统一下单的签名类型一致
        public string signType { get; set; }
        // 签名
        public string paySign { get; set; }
        // 统一下单返回信息，如非空，为错误原因
        public string msg { get; set; }
        //  SUCCESS/FAIL 此字段是通信标识，非交易标识，交易是否成功需要查看result_code来判断
        public string code { get; set; }
        // 网站订单id
        public string orderId { get; set; }
    }
}