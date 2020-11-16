using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WebApi.Models.Request.Mini
{
    /// <summary>
    /// 统一下单
    /// </summary>
    public class Pay
    {
        /// <summary>
        /// 小程序登录API
        /// </summary>
        public static string loginUrl = ConfigurationManager.AppSettings["loginurl"].ToString();

        /// <summary>
        /// 统一下单API
        /// </summary>
        public static string orderUrl = ConfigurationManager.AppSettings["orderurl"].ToString();

        /// <summary>
        /// 支付结果通知API
        /// </summary>
        public static string notifyUrl = "/pay/OrderNotify";

        /// <summary>
        /// 查询订单API
        /// </summary>
        public static string queryUrl = ConfigurationManager.AppSettings["queryurl"].ToString();

        /// <summary>
        /// 申请退款API
        /// </summary>
        public static string refundUrl = ConfigurationManager.AppSettings["refundurl"].ToString();

        /// <summary>
        /// 退款通知API
        /// </summary>
        public static string refundNotifyUrl = ConfigurationManager.AppSettings["refundnotifyurl"].ToString();

        /// <summary>
        /// 退款通知API
        /// </summary>
        public static string refundQueryUrl = ConfigurationManager.AppSettings["refundqueryurl"].ToString();

        /// <summary>
        /// 小程序唯一标识
        /// </summary>
        public static string appid = ConfigurationManager.AppSettings["appid"].ToString();

        /// <summary>
        /// 小程序的 app secret
        /// </summary>
        public static string secret = ConfigurationManager.AppSettings["secret"].ToString();

        /// <summary>
        /// 小程序的授权类型 
        /// </summary>
        public static string grantType = ConfigurationManager.AppSettings["grant_type"].ToString();

        /// <summary>
        /// 商户号(微信支付分配的商户号)
        /// </summary>
        public static string mchid = ConfigurationManager.AppSettings["mch_id"].ToString();

        /// <summary>
        ///商户平台设置的密钥key
        /// </summary>
        public static string key = ConfigurationManager.AppSettings["key"].ToString();

        /// <summary>
        /// 随机字符串不长于 32 位
        /// </summary>
        //public static string nonceStr = Helper.RandomHelper.GetCodeStr(20);

        /// <summary>
        /// 时间戳 从1970年1月1日00:00:00至今的秒数,即当前的时间
        /// </summary>
        //public static string timeStamp = PayHelper.GetTimeStamp();

        /// <summary>
        /// 终端IP APP和网页支付提交用户端IP，
        /// </summary>
        //public static string addrIp = PayHelper.GetIP;

        /// <summary>
        /// 交易类型 小程序取值如下：JSAPI
        /// </summary>
        public static string tradeType = "JSAPI";

        /// <summary>
        /// 签名类型 默认为MD5，支持HMAC-SHA256和MD5。
        /// </summary>
        public static string signType = "MD5";

        /// <summary>
        /// 商品描述 商品简单描述，该字段请按照规范传递
        /// </summary>
        public static string body = "起飞竞球-文章购买";

        /// <summary>
        /// 附加数据 在查询API和支付通知中原样返回
        /// </summary>
        public static string attach = "微信支付信息";

        /// <summary>
        /// 签名，参与签名参数：appid，mch_id，transaction_id，out_trade_no，nonce_str，key
        /// </summary>
        public string sign = "";

        /// <summary>
        /// 微信订单号，优先使用
        /// </summary>
        public static string transactionid = "";

        /// <summary>
        /// 商户系统内部订单号
        /// </summary>
        public static string out_trade_no = "";

        /// <summary>
        /// 商户退款单号
        /// </summary>
        public static string out_refund_no = "";

        /// <summary>
        /// 退款金额
        /// </summary>
        public static decimal refundfee;

        /// <summary>
        /// 订单金额
        /// </summary>
        public static decimal totalfee;
    }

    /// <summary>
    /// 生成订单信息
    /// </summary>
    public class PayOrder
    {
        // 买家openId
        public string openId { get; set; }
        // 订单号
        public Guid orderNum { get; set; }
        // 订单金额
        public decimal orderTotal { get; set; }
    }
}