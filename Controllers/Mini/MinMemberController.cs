﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MRequest = WebApi.Models.Request.Mini;
using MResponse = WebApi.Models.Response.Mini;


namespace WebApi.Controllers.Mini
{
    public class MinMemberController : ApiController
    {
        private MiniDB.MiniDB DB = new MiniDB.MiniDB();
        private readonly string Appid = "wx1148378bc5c4f41d";
        private readonly string Secret = "b0803d1019699d4b5f8f900ca061d83d";
        private readonly string grant_type = "authorization_code";


        /// <summary>
        /// 登录
        /// 返回token和uid
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Login(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return Json(new { statusCode = HttpStatusCode.BadRequest, msg = "请求失败" });
            }
            //向微信服务端 使用登录凭证 code 获取 session_key 和 openid   
            // https://api.weixin.qq.com/sns/jscode2session?appid=APPID&secret=SECRET&js_code=JSCODE&grant_type=authorization_code

            string url = "https://api.weixin.qq.com/sns/jscode2session?appid=" + Appid + "&secret=" + Secret + "&js_code=" + code + "&grant_type=" + grant_type;

            Helper.MiniLoginHelper miniLoginHelper = new Helper.MiniLoginHelper();
            string loginResponse = miniLoginHelper.GetUrltoHtml(url, "utf-8");//获取微信服务器返回字符串  

            //将字符串转换为json格式  
            var wxLoginRes = Helper.JsonHelper.DeserializeJsonToObject<MResponse.WxLogin>(loginResponse);
            if (wxLoginRes == null)
            {
                return Json(new { statusCode = HttpStatusCode.InternalServerError, msg = "微信登录失败" });
            }


            if (!string.IsNullOrEmpty(wxLoginRes.openid))
            {
                var member = DB.Member.FirstOrDefault(m => m.OpenId == wxLoginRes.openid);
                if (member == null)
                {
                    return Json(new { statusCode = HttpStatusCode.NoContent, msg = "用户不存在" });
                }
                return Json(new
                {
                    statusCode = HttpStatusCode.OK,
                    content = new
                    {
                        token = Helper.EncryptionHelper.CreateToken(member.Id, "980323", 9999000),
                        uid = member.Id
                    }
                });
            }
            else
            {
                return Json(new { statusCode = HttpStatusCode.InternalServerError, msg = "微信登录失败" });
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetUserInfo(Guid uid)
        {
            if (uid == Guid.Empty)
            {
                return Json(new { statusCode = HttpStatusCode.BadRequest, msg = "请求失败" });
            }
            var member = await DB.Member.FindAsync(uid);
            if (member == null)
            {
                return Json(new { statusCode = HttpStatusCode.NoContent, msg = "用户不存在" });
            }
            return Json(new
            {
                statusCode = HttpStatusCode.OK,
                content = new
                {
                    nickName = member.NickName,
                    phone = member.Phone,
                    maker = member.Maker,
                    expert = member.Expert,
                    avatarUrl = member.AvatarUrl
                }
            });
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="signUp"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> SignUp(MRequest.SignUp signUp)
        {
            if (ModelState.IsValid)
            {
                //向微信服务端 使用登录凭证 code 获取 session_key 和 openid   
                // https://api.weixin.qq.com/sns/jscode2session?appid=APPID&secret=SECRET&js_code=JSCODE&grant_type=authorization_code

                string url = "https://api.weixin.qq.com/sns/jscode2session?appid=" + Appid + "&secret=" + Secret + "&js_code=" + signUp.code + "&grant_type=" + grant_type;

                Helper.MiniLoginHelper miniLoginHelper = new Helper.MiniLoginHelper();
                string loginResponse = miniLoginHelper.GetUrltoHtml(url, "utf-8");//获取微信服务器返回字符串  

                //将字符串转换为json格式  
                var wxLoginRes = Helper.JsonHelper.DeserializeJsonToObject<MResponse.WxLogin>(loginResponse);
                if (wxLoginRes == null)
                {
                    return Json(new { statusCode = HttpStatusCode.InternalServerError, msg = "微信登录失败" });
                }


                if (!string.IsNullOrEmpty(wxLoginRes.openid))
                {
                    //用户数据解密  
                    miniLoginHelper.AesIV = signUp.iv;
                    miniLoginHelper.AesKey = wxLoginRes.session_key;

                    string userInfo = miniLoginHelper.AESDecrypt(signUp.encryptedData);


                    //存储用户数据  
                    var wxUserInfoRes = Helper.JsonHelper.DeserializeJsonToObject<MResponse.WxUserInfo>(userInfo);
                    if (wxUserInfoRes == null)
                    {
                        return Json(new { statusCode = HttpStatusCode.InternalServerError, msg = "信息获取失败" });
                    }
                    if (DB.Member.FirstOrDefault(m => m.OpenId == wxLoginRes.openid) != null)
                    {
                        return Json(new { statusCode = HttpStatusCode.BadRequest, msg = "用户已存在" });
                    }
                    var memberDB = new MiniDB.Member
                    {
                        Id = Guid.NewGuid(),
                        OpenId = wxUserInfoRes.openId,
                        NickName = wxUserInfoRes.nickName,
                        Gender = wxUserInfoRes.gender,
                        City = wxUserInfoRes.city,
                        Province = wxUserInfoRes.province,
                        Country = wxUserInfoRes.country,
                        AvatarUrl = wxUserInfoRes.avatarUrl,
                        UnionId = wxUserInfoRes.unionId,
                        SessionKey = wxLoginRes.session_key,
                        Time = DateTime.Now
                    };
                    try
                    {
                        DB.Member.Add(memberDB);
                        await DB.SaveChangesAsync();
                        return Json(new { statusCode = HttpStatusCode.Created, msg = "注册成功" });
                    }
                    catch
                    {
                        return Json(new { statusCode = HttpStatusCode.InternalServerError, msg = "注册失败" });
                    }

                }
                else
                {
                    return Json(new { statusCode = HttpStatusCode.InternalServerError, msg = "微信登录失败" });
                }
            }
            else
            {
                return Json(new { statusCode = HttpStatusCode.BadRequest, msg = "请求失败" });
            }


        }


    }
}
