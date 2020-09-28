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
using System.Web;
using ResMember = WebApi.Models.Response.Web.Member;

namespace WebApi.Controllers.Web
{
    public class MemberController : ApiController
    {
        private readonly DataBase.DB db = new DataBase.DB();
        private const string TOKEN = "ZFDYES";

        /// <summary>
        /// 获取RSA公钥
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetPublicKey()
        {
            return Json(new {status ="success",msg="获取成功",content = ConfigurationManager.AppSettings["RSAPublic"].ToString() });
            //return Json(new { status = "success",msg = "获取成功",content= ConfigurationManager.AppSettings["RSAPublic"].ToString() });
        }
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Login(ReqMem.Login member)
        {
            if (ModelState.IsValid)
            {
                Helper.RSACryptoService rsaCryptoService = new Helper.RSACryptoService(ConfigurationManager.AppSettings["RSAPrivate"].ToString(), ConfigurationManager.AppSettings["RSAPublic"].ToString());
                DataBase.Members mem = db.Members.FirstOrDefault(m => m.Name == member.UserName);
                if (mem == null || mem.Status != Models.Config.Status.normal)
                {
                    return Json(new { status = "fail", msg = "用户不存在或已被禁用" });
                }
                string password = rsaCryptoService.Decrypt(member.Password);
                if(!Helper.EncryptionHelper.SHA1(password + mem.PasswordSalt, Encoding.UTF8, false).Equals(mem.Password))
                {
                    return Json(new FailResponse("用户名密码错误"));
                }
                return Json(new { status = "success", msg = "登录成功", content = Helper.EncryptionHelper.CreateToken(mem.Id, mem.PasswordSalt) });
            }
            else
            {
                return Json(new { status = "fail", msg = "请求参数格式错误" });
            }

        }
        /// <summary>
        /// 获取用户的基础信息 + role + org
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> GetLoginerInfo()
        {
            Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);
            if (userId == Guid.Empty)
            {
                return Json(new { status = "fail", msg = "请求错误" });
            }
            var member = await db.Members.FindAsync(userId);
            if (member == null || member.Status != Models.Config.Status.normal)
            {
                return Json(new { status = "fail", msg = "用户不存在或已被禁用" });
            }
            var loginInfo = db.Members.Where(m => m.Id == userId).Select(m => new { 
                m.Name,
                m.NickName,
                m.Phone,
                m.Avatar,
                Orgs = db.MemOrg.Where(mo=>mo.Member == userId && mo.Orgs.Status == Models.Config.Status.normal).Select(mo => new { 
                    mo.Orgs.Id,
                    mo.Orgs.Name
                }),
                Roles = db.MemRole.Where(mr=>mr.Member == userId && mr.Roles.Status == Models.Config.Status.normal).Select(mr => new { 
                    mr.Roles.Id,
                    mr.Roles.Name
                })
            }).FirstOrDefault();
            return Json(new { status = "success", msg = "获取成功", content = loginInfo });
        }
        [HttpGet]
        public async Task<IHttpActionResult> ListOrgMember(Guid orgId,int pi,int ps)
        {
            Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);
            if (userId == Guid.Empty)
            {
                return Json(new { status = "fail", msg = "请求错误" });
            }
            var member = await db.Members.FindAsync(userId);
            if (member == null || member.Status != Models.Config.Status.normal)
            {
                return Json(new { status = "fail", msg = "用户不存在或已被禁用" });
            }
            var orgs = db.MemOrg.Where(mo => mo.Member == userId).Select(mo => mo.Org);
            if (orgs.Count() == 0)
            {
                return Json(new { status = "fail", msg = "部门为空" });
            }
            if (!orgs.Contains(orgId))
            {
                return Json(new { status = "fail", msg = "部门不存在或无权限" });
            }
            var members = db.MemOrg.Where(mo => mo.Org == orgId).Select(mo2 => new ResMember.OrgMember
            {
                Id = mo2.Member,
                Name = mo2.Members.Name,
                Phone = mo2.Members.Phone,
                Roles = db.MemRole.Where(mr => mr.Member == mo2.Member).Select(mr => mr.Roles.Name).ToArray()
            });

            var pagination = Helper.PaginationHelper<ResMember.OrgMember>.Paging(members, pi, ps);
            return Json(new { status = "success", msg = "获取成功",content = pagination });

        }
    }
}
