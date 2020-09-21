using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ReqMem = WebApi.Models.Request.Web.Member;
using WebApi.Models.Response;
using System.Threading.Tasks;
using System.Text;

namespace WebApi.Controllers.Web
{
    public class MemberController : ApiController
    {
        private readonly DataBase.DB db = new DataBase.DB();

        /// <summary>
        /// 获取RSA公钥
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetPublicKey()
        {
            return Json(new SuccessResponse("获取成功", ConfigurationManager.AppSettings["RSAPublic"].ToString()));
            //return Json(new { status = "success",msg = "获取成功",content= ConfigurationManager.AppSettings["RSAPublic"].ToString() });
        }
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public IHttpActionResult Login(ReqMem.Login member)
        {
            if (ModelState.IsValid)
            {
                Helper.RSACryptoService rsaCryptoService = new Helper.RSACryptoService(ConfigurationManager.AppSettings["RSAPrivate"].ToString(), ConfigurationManager.AppSettings["RSAPublic"].ToString());
                DataBase.Members mem = db.Members.FirstOrDefault(m => m.Name == member.UserName);
                if (mem == null)
                {
                    return Json(new FailResponse("用户不存在"));
                }
                string password = rsaCryptoService.Decrypt(member.Password);
                if(!Helper.EncryptionHelper.SHA1(password + mem.PasswordSalt, Encoding.UTF8, false).Equals(mem.Password))
                {
                    return Json(new FailResponse("用户名密码错误"));
                }
                return Json(new SuccessResponse("登录成功", new { token = Helper.EncryptionHelper.CreateToken(mem.Id, mem.PasswordSalt),id = mem.Id}));
            }
            else
            {
                return Json(new FailResponse("请求参数格式错误"));
            }

        }

    }
}
