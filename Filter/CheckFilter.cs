using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace WebApi.Filter
{
    public class CheckFilter : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            try
            {
                if (actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any())
                {
                    return;
                }
                Guid userId = Guid.Parse(actionContext.Request.Headers.GetValues("sessionId").First());
                string token = actionContext.Request.Headers.GetValues("X-Token").First();
                var user = new DataBase.DB().Member.Find(userId);
                //token校验成功
                if (user.Status == (short)Models.Setting.NormalStauts.正常 && Helper.EncryptionHelper.CheckToken(token, userId, user.PasswordSalt))
                {
                    return;
                }
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new { status = "fail", msg = "无权限" });
            }
            catch
            {
                HttpRequest request = HttpContext.Current.Request;
                string msg = string.Format("请求主机：{0}\n请求代理：{1}\n请求方法：{2}\n请求IP：{3}\n请求文本类型：{4}",
                   request.UserHostAddress, request.UserAgent,request.HttpMethod,request.RawUrl,request.ContentType);
                Helper.LogHelper.WriteUnAuthorizationLog(msg);
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new { status = "fail", msg = "无权限" });
            }
        }
    }
}