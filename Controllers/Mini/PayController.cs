using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using PayInfo = WebApi.Models.Request.Mini.Pay;
using EntityReq = WebApi.Models.Request.Mini;
using EntityRes = WebApi.Models.Response.Mini;
using System.Threading.Tasks;

namespace WebApi.Controllers.Mini
{
    public class PayController : ApiController
    {
        private readonly MiniDB.MiniDB DB = new MiniDB.MiniDB();

        /// <summary>
        /// 统一下单,返回下单信息给用户。用户发起支付请求，确认支付
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult Pay(EntityReq.PayOrder pay)
        {
            //获取请求数据
            Dictionary<string, string> strParam = new Dictionary<string, string>();
            //小程序ID
            strParam.Add("appid", PayInfo.appid);
            //附加数据
            strParam.Add("attach", PayInfo.attach);
            //商品描述
            strParam.Add("body", PayInfo.body);
            //商户号
            strParam.Add("mch_id", PayInfo.mchid); // 暂未
            //随机字符串
            strParam.Add("nonce_str", Helper.RandomHelper.GetCodeStr(20));
            //通知地址 (异步接收微信支付结果通知的回调地址，通知url必须为外网可访问的url，不能携带参数。)
            strParam.Add("notify_url", HttpContext.Current.Request.ApplicationPath + PayInfo.notifyUrl);
            //用户标识
            strParam.Add("openid", pay.openId);
            //商户订单号
            strParam.Add("out_trade_no", pay.orderNum.ToString("N"));
            //用户终端IP
            strParam.Add("spbill_create_ip", HttpContext.Current.Request.UserHostAddress);

            //标价金额
            strParam.Add("total_fee", Convert.ToInt32(pay.orderTotal * 100).ToString());
            //交易类型
            strParam.Add("trade_type", PayInfo.tradeType);
            strParam.Add("sign", Helper.WxPayHelper.GetSignInfo(strParam, PayInfo.key)); // 暂未
            //获取预支付ID
            string preInfo = PostHttpResponse(PayInfo.orderUrl, Helper.WxPayHelper.CreateXmlParam(strParam));

            //  SUCCESS/FAIL 此字段是通信标识，非交易标识，交易是否成功需要查看result_code来判断
            string strCode = Helper.WxPayHelper.GetXmlValue(preInfo, "return_code");
            // 返回信息，如非空，为错误原因
            string strMsg = Helper.WxPayHelper.GetXmlValue(preInfo, "return_msg");
            var info = new EntityRes.Pay();

            if (strCode == "SUCCESS")
            {
                //再次签名
                string nonecStr = Helper.RandomHelper.GetCodeStr(20);
                string timeStamp = Helper.TypeHelper.GetTimeStamp();
                string package = "prepay_id=" + Helper.WxPayHelper.GetXmlValue(preInfo, "prepay_id");
                Dictionary<string, string> singInfo = new Dictionary<string, string>();
                singInfo.Add("appId", PayInfo.appid);
                singInfo.Add("nonceStr", nonecStr);
                singInfo.Add("package", package);
                singInfo.Add("signType", PayInfo.signType);
                singInfo.Add("timeStamp", timeStamp);
                //返回参数
                info.msg = strMsg;
                info.code = strCode;
                //info.Id = orderId;
                info.appId = PayInfo.appid;
                info.orderId = pay.orderNum.ToString("N");
                info.package = package;
                info.timeStamp = timeStamp;
                info.nonceStr = nonecStr;
                info.signType = PayInfo.signType;
                info.package = Helper.WxPayHelper.GetSignInfo(singInfo, PayInfo.key);
                return Json(new { statusCode = HttpStatusCode.OK, msg = "下单成功", content = info });

            }
            else
            {
                info.code = strCode;
                info.msg = strMsg;
                return Json(new { statusCode = HttpStatusCode.BadRequest, msg = "下单失败",content=info });

            }

        }

        /// <summary>
        /// 异步接收微信回传的用户支付信息，更新商家订单并返回结果回微信服务器
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> OrderNotify()
        {
            string strResult = string.Empty;
            try
            {
                //1.获取微信通知的参数
                string strXML = GetPostStr();
                //判断是否请求成功
                if (Helper.WxPayHelper.GetXmlValue(strXML, "return_code") == "SUCCESS")
                {
                    //判断是否支付成功
                    if (Helper.WxPayHelper.GetXmlValue(strXML, "result_code") == "SUCCESS")
                    {
                        //获得签名
                        string getSign = Helper.WxPayHelper.GetXmlValue(strXML, "sign");
                        //进行签名
                        string sign = Helper.WxPayHelper.GetSignInfo(Helper.WxPayHelper.GetFromXml(strXML), PayInfo.key);
                        if (sign == getSign)
                        {
                            //校验订单信息
                            string wxOrderNum = Helper.WxPayHelper.GetXmlValue(strXML, "transaction_id"); //微信订单号
                            string orderNum = Helper.WxPayHelper.GetXmlValue(strXML, "out_trade_no");    //商户订单号
                            string orderTotal = Helper.WxPayHelper.GetXmlValue(strXML, "total_fee");
                            string openid = Helper.WxPayHelper.GetXmlValue(strXML, "openid");
                            var orderDb = await DB.Order.FindAsync(Guid.Parse(orderNum));
                            //校验订单是否存在
                            if (orderDb != null)
                            {
                                //2.更新订单的相关状态
                                orderDb.Status = Models.Config.Status.normal;
                                orderDb.TimeExpire = DateTime.Now;
                                //3.返回一个xml格式的结果给微信服务器
                                if (await DB.SaveChangesAsync() > 0)
                                {
                                    strResult = Helper.WxPayHelper.GetReturnXml("SUCCESS", "OK");
                                }
                                else
                                {
                                    strResult = Helper.WxPayHelper.GetReturnXml("FAIL", "订单状态更新失败");
                                }
                            }
                            else
                            {
                                strResult = Helper.WxPayHelper.GetReturnXml("FAIL", "支付结果中微信订单号数据库不存在！");
                            }
                        }
                        else
                        {
                            strResult = Helper.WxPayHelper.GetReturnXml("FAIL", "签名不一致!");
                        }
                    }
                    else
                    {
                        strResult = Helper.WxPayHelper.GetReturnXml("FAIL", "支付通知失败!");
                    }
                }
                else
                {
                    strResult = Helper.WxPayHelper.GetReturnXml("FAIL", "支付通知失败!");
                }

            }
            catch (Exception ex)
            {

            }
            return Json(strResult);
        }


        /// <summary>
        /// 获得Post过来的数据  
        /// </summary>
        /// <returns></returns>
        public static string GetPostStr()
        {
            Int32 intLen = Convert.ToInt32(HttpContext.Current.Request.InputStream.Length);
            byte[] b = new byte[intLen];
            HttpContext.Current.Request.InputStream.Read(b, 0, intLen);
            return Encoding.UTF8.GetString(b);
        }

        /// <summary>  
        /// 模拟POST提交  
        /// </summary>  
        /// <param name="url">请求地址</param>  
        /// <param name="xmlParam">xml参数</param>  
        /// <returns>返回结果</returns>  
        public static string PostHttpResponse(string url, string xmlParam)
        {
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            myHttpWebRequest.Method = "POST";
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8";

            // Encode the data  
            byte[] encodedBytes = Encoding.UTF8.GetBytes(xmlParam);
            myHttpWebRequest.ContentLength = encodedBytes.Length;

            // Write encoded data into request stream  
            Stream requestStream = myHttpWebRequest.GetRequestStream();
            requestStream.Write(encodedBytes, 0, encodedBytes.Length);
            requestStream.Close();

            HttpWebResponse result;

            try
            {
                result = (HttpWebResponse)myHttpWebRequest.GetResponse();
            }
            catch
            {
                return string.Empty;
            }

            if (result.StatusCode == HttpStatusCode.OK)
            {
                using (Stream mystream = result.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(mystream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            return null;
        }
    }
}
