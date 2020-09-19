using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using DataBase;
using RequestModel = WebApi.Models.Request.Mini.Member;

namespace WebApi.Controllers.Mini
{
    public class MemberController : ApiController
    {
        private readonly DB db = new DB();

        //[Filter.AuthorityFilter("0")]
        //        [AllowAnonymous]
        //[HttpPost]
        public IHttpActionResult Login(RequestModel.Login login)
        {
            if (ModelState.IsValid)
            {
                Members loginer = db.Members.AsNoTracking().Where(p => p.Name == login.Username && p.Status == 0).FirstOrDefault();
                if (loginer == null)
                {
                    return Json(new { status = "fail", msg = "用户不存在" });
                }
                if (!loginer.Password.Equals(Helper.EncryptionHelper.SHA1(login.Password + loginer.PasswordSalt, Encoding.UTF8, false)))
                {
                    return Json(new { status = "fail", msg = "用户名密码错误" });
                }
                return Json(new
                {
                    status = "success",
                    msg = "登录成功",
                    content = new { token = Helper.EncryptionHelper.CreateToken(loginer.Id, loginer.PasswordSalt), userId = loginer.Id }
                });
            }
            else
            {
                return Json(new { status = "fail", meg = "请求无效", content = ModelState });
                //return BadRequest(ModelState);
            }
        }
    }
}
