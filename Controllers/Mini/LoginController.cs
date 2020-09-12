using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using DataBase;
using LoginModel = WebApi.Models.Request.Mini.Login;

namespace WebApi.Controllers.Mini
{
    public class LoginController : ApiController
    {
        private DB db = new DB();

        public IHttpActionResult Login(LoginModel.LoginModel login)
        {
            if (ModelState.IsValid)
            {
                Member loginer = db.Member.AsNoTracking().Where(p => p.Name == login.Username && p.Status == 0).FirstOrDefault();
                if (loginer == null)
                {
                    return Json(new { code = 404, msg = "用户不存在" });
                }
                if (!loginer.Password.Equals(Helper.EncryptionHelper.SHA1(login.Password + loginer.PasswordSalt, Encoding.UTF8, false)))
                {
                    return Json(new { code = 203, msg = "用户名密码错误" });
                }
                return Json(new { code = 200, msg = "登录成功", content = Helper.EncryptionHelper.CreateToken(loginer.Id, loginer.PasswordSalt) });
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}
